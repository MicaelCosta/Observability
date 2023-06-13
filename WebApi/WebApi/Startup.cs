using Application.Commands.CreatePedido;
using Application.Consumers;
using Core.Caching;
using Core.Repositories;
using Infrastructure.Caching;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Database");
            services.AddDbContext<ContextCore>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IPedidoProdutoRepository, PedidoProdutoRepository>();
            services.AddScoped<ICachingService, CachingService>();

            services.AddControllers();

            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(typeof(CreatePedidoCommand).Assembly); });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });

            IConnectionMultiplexer redisConnectionMultiplexer = ConnectionMultiplexer.ConnectAsync(Configuration.GetValue<string>("Redis:Url") ?? string.Empty).Result;
            services.AddSingleton(redisConnectionMultiplexer);
            services.AddStackExchangeRedisCache(o =>
            {
                o.InstanceName = "MicaCake";
                o.ConnectionMultiplexerFactory = () => Task.FromResult(redisConnectionMultiplexer);
            });

            AddOpenTelemetry(services);

            AddMassTransitExtension(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureResourceOpenTelemetry(ResourceBuilder r) => r
            .AddService(serviceName: Configuration.GetValue<string>("DistributedTracing:Jaeger:ServiceName") ?? string.Empty,
            serviceVersion: typeof(Startup).Assembly.GetName().Version?.ToString() ?? "unknown",
            serviceInstanceId: Environment.MachineName)
            .AddTelemetrySdk()
            .AddEnvironmentVariableDetector()
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment.name"] = "development",
                ["team.name"] = "MicaCake Backend"
            });

        private void AddOpenTelemetry(IServiceCollection services)
        {
            services
            .AddOpenTelemetry()
            .ConfigureResource(ConfigureResourceOpenTelemetry)
            .WithTracing(builder =>
            {
                builder.AddSource(Configuration.GetValue<string>("DistributedTracing:Jaeger:ServiceName"))
                    .AddAspNetCoreInstrumentation(p =>
                    {
                        p.RecordException = true;
                        p.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.AddTag("QUERY_STRING_CUSTOM", httpRequest.QueryString.ToString());
                        };
                        p.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.AddTag("CONTENT_TYPE_CUSTOM", httpResponse.ContentType);
                        };
                        p.EnrichWithException = (activity, exception) =>
                        {
                            activity.AddTag("EXCEPTION_MESSAGE_CUSTOM", exception.Message);
                        };
                    })
                    .AddHttpClientInstrumentation(p =>
                    {
                        p.RecordException = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation(p =>
                    {
                        p.SetDbStatementForText = true;
                        p.SetDbStatementForStoredProcedure = true;
                        p.EnrichWithIDbCommand = (activity, command) =>
                        {
                            activity.IsAllDataRequested = true;
                            activity.SetTag("COMMANDTEXT_CUSTOM", command.CommandText);
                            activity.SetTag("COMMANDTIMEOUT_CUSTOM", command.CommandTimeout);
                            activity.SetTag("COMMANDCONNECTION_CUSTOM", command.Connection?.ConnectionString);
                        };
                    })
                    .AddRedisInstrumentation(p =>
                    {
                        p.SetVerboseDatabaseStatements = true;
                        p.FlushInterval = TimeSpan.FromSeconds(1);
                    })
                    .AddMassTransitInstrumentation()
                    .SetSampler(new AlwaysOnSampler())
                    .AddJaegerExporter(p =>
                    {
                        p.AgentHost = Configuration.GetValue<string>("DistributedTracing:Jaeger:Host");
                        p.AgentPort = Configuration.GetValue<int>("DistributedTracing:Jaeger:Port");
                    });
            });

            services
            .AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddOpenTelemetry(options =>
                {
                    options.AddConsoleExporter()
                           .SetResourceBuilder(ResourceBuilder.CreateDefault()
                                                              .AddService(Configuration.GetValue<string>("DistributedTracing:Jaeger:ServiceName") ?? string.Empty));
                });
            });

            services.Configure<OpenTelemetryLoggerOptions>(opt =>
            {
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.IncludeFormattedMessage = true;
            });
        }

        private void AddMassTransitExtension(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddDelayedMessageScheduler();
                x.SetKebabCaseEndpointNameFormatter();

                x.AddConsumer<PedidoConsumer>(typeof(PedidoConsumerDefinition));

                x.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(Configuration.GetValue<string>("RabbitMq:Url") ?? string.Empty);

                    cfg.UseDelayedMessageScheduler();
                    cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter("dev", false));
                    cfg.UseMessageRetry(retry => { retry.Interval(3, TimeSpan.FromSeconds(5)); });
                });
            });
        }
    }
}
