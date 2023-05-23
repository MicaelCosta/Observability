using Application.Commands.CreatePedido;
using Core.Repositories;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

            services.AddControllers();

            services.AddMediatR(cfg => { cfg.RegisterServicesFromAssemblies(typeof(CreatePedidoCommand).Assembly); });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
            });

            AddOpenTelemetry(services);
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
            .WithTracing(p =>
            {
                p.AddSource(Configuration.GetValue<string>("DistributedTracing:Jaeger:ServiceName"))
                    .AddAspNetCoreInstrumentation(p =>
                    {
                        p.RecordException = true;
                    })
                    .AddHttpClientInstrumentation(p =>
                    {
                        p.RecordException = true;
                    })
                    .AddSqlClientInstrumentation(p =>
                    {
                        p.SetDbStatementForText = true;
                        p.EnableConnectionLevelAttributes = true;
                        p.RecordException = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation(p =>
                    {
                        p.SetDbStatementForText = true;
                    })
                    .SetSampler(new AlwaysOnSampler())
                    .AddJaegerExporter(p =>
                    {
                        p.AgentHost = Configuration.GetValue<string>("DistributedTracing:Jaeger:Host");
                        p.AgentPort = Configuration.GetValue<int>("DistributedTracing:Jaeger:Port");
                    });
            });

            services
            .AddLogging(build =>
            {
                build.SetMinimumLevel(LogLevel.Debug);
                build.AddOpenTelemetry(options =>
                {
                    options.AddConsoleExporter().SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(Configuration.GetValue<string>("DistributedTracing:Jaeger:ServiceName") ?? string.Empty));
                });
            });

            services.Configure<AspNetCoreInstrumentationOptions>(options => options.RecordException = true);
            services.Configure<OpenTelemetryLoggerOptions>(opt =>
            {
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.IncludeFormattedMessage = true;
            });
        }
    }
}
