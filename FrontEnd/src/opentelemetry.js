//import { trace, context, SpanStatusCode } from "@opentelemetry/api";
import {
  BatchSpanProcessor,
  WebTracerProvider,
} from "@opentelemetry/sdk-trace-web";
//import { getWebAutoInstrumentations } from "@opentelemetry/auto-instrumentations-web";
import { OTLPTraceExporter } from "@opentelemetry/exporter-trace-otlp-http";
//import { BatchSpanProcessor } from "@opentelemetry/sdk-trace-base";
//import { registerInstrumentations } from "@opentelemetry/instrumentation";
//import { ZoneContextManager } from "@opentelemetry/context-zone";
//import { Resource } from "@opentelemetry/resources";

const collectorOptions = {
  url: "http://localhost:4318/v1/traces", // url is optional and can be omitted - default is http://localhost:4318/v1/traces
  headers: {}, // an optional object containing custom headers to be sent with each request
  concurrencyLimit: 10, // an optional limit on pending requests
};

const provider = new WebTracerProvider();
const exporter = new OTLPTraceExporter(collectorOptions);
provider.addSpanProcessor(
  new BatchSpanProcessor(exporter, {
    // The maximum queue size. After the size is reached spans are dropped.
    maxQueueSize: 100,
    // The maximum batch size of every export. It must be smaller or equal to maxQueueSize.
    maxExportBatchSize: 10,
    // The interval between two consecutive exports
    scheduledDelayMillis: 500,
    // How long the export can run before it is cancelled
    exportTimeoutMillis: 30000,
  })
);

provider.register();
