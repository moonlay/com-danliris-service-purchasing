using AutoMapper;
using Com.DanLiris.Service.Purchasing.Lib;
using Com.DanLiris.Service.Purchasing.Lib.Facades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.BankExpenditureNoteFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.Expedition;
using Com.DanLiris.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.DanLiris.Service.Purchasing.Lib.Facades.ExternalPurchaseOrderFacade.Reports;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentBeacukaiFacade;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentCorrectionNoteFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentDeliveryOrderFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentInternalPurchaseOrderFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentInternNoteFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentInvoiceFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentPurchaseRequestFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentUnitReceiptNoteFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.InternalPO;
using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringUnitReceiptFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.PurchasingDispositionFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.Report;
using Com.DanLiris.Service.Purchasing.Lib.Facades.UnitPaymentCorrectionNoteFacade;
using Com.DanLiris.Service.Purchasing.Lib.Facades.UnitReceiptNoteFacade;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentUnitExpenditureNoteFacade;
using Com.DanLiris.Service.Purchasing.Lib.Helpers;
using Com.DanLiris.Service.Purchasing.Lib.Interfaces;
using Com.DanLiris.Service.Purchasing.Lib.Serializers;
using Com.DanLiris.Service.Purchasing.Lib.Services;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.IntegrationViewModel;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.PurchaseOrder;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.UnitPaymentCorrectionNoteViewModel;
using Com.DanLiris.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using Com.DanLiris.Service.Purchasing.WebApi.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json.Serialization;
using System.Text;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentExternalPurchaseOrderFacade.Reports;
using Com.DanLiris.Service.Purchasing.Lib.Facades.PurchaseRequestFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentUnitDeliveryOrderReturFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentReceiptCorrectionFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCentralBillReceptionFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCentralBillExpenditureFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteReceptionFacades;
using FluentScheduler;
using Com.DanLiris.Service.Purchasing.WebApi.SchedulerJobs;
using Com.DanLiris.Service.Purchasing.Lib.Utilities.CacheManager;
using Com.DanLiris.Service.Purchasing.Lib.Facades.MonitoringCorrectionNoteExpenditureFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentPOMasterDistributionFacades;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentReports;
using Com.DanLiris.Service.Purchasing.Lib.Utilities.Currencies;
using Com.DanLiris.Service.Purchasing.Lib.Facades.GarmentDailyPurchasingReportFacade;
using Com.DanLiris.Service.Purchasing.Lib.AutoMapperProfiles;
using Com.DanLiris.Service.Purchasing.Lib.Utilities;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights.AspNetCore;

namespace Com.DanLiris.Service.Purchasing.WebApi
{
    public class Startup
    {
        /* Hard Code */
        private string[] EXPOSED_HEADERS = new string[] { "Content-Disposition", "api-version", "content-length", "content-md5", "content-type", "date", "request-id", "response-time" };
        private string PURCHASING_POLICITY = "PurchasingPolicy";

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region Register

        private void RegisterEndpoints()
        {
            APIEndpoint.Purchasing = Configuration.GetValue<string>(Constant.PURCHASING_ENDPOINT) ?? Configuration[Constant.PURCHASING_ENDPOINT];
            APIEndpoint.Core = Configuration.GetValue<string>(Constant.CORE_ENDPOINT) ?? Configuration[Constant.CORE_ENDPOINT];
            APIEndpoint.Inventory = Configuration.GetValue<string>(Constant.INVENTORY_ENDPOINT) ?? Configuration[Constant.INVENTORY_ENDPOINT];
            APIEndpoint.Finance = Configuration.GetValue<string>(Constant.FINANCE_ENDPOINT) ?? Configuration[Constant.FINANCE_ENDPOINT];
            APIEndpoint.CustomsReport = Configuration.GetValue<string>(Constant.CUSTOMSREPORT_ENDPOINT) ?? Configuration[Constant.FINANCE_ENDPOINT];
            APIEndpoint.Sales = Configuration.GetValue<string>(Constant.SALES_ENDPOINT) ?? Configuration[Constant.SALES_ENDPOINT];
            APIEndpoint.Auth = Configuration.GetValue<string>(Constant.AUTH_ENDPOINT) ?? Configuration[Constant.AUTH_ENDPOINT];
            APIEndpoint.GarmentProduction = Configuration.GetValue<string>(Constant.GARMENT_PRODUCTION_ENDPOINT) ?? Configuration[Constant.GARMENT_PRODUCTION_ENDPOINT];

            AuthCredential.Username = Configuration.GetValue<string>(Constant.USERNAME) ?? Configuration[Constant.USERNAME];
            AuthCredential.Password = Configuration.GetValue<string>(Constant.PASSWORD) ?? Configuration[Constant.PASSWORD];
        }

        private void RegisterFacades(IServiceCollection services)
        {
            services
                .AddTransient<ICoreData, CoreData>()
                .AddTransient<ICoreHttpClientService, CoreHttpClientService>()
                .AddTransient<IMemoryCacheManager, MemoryCacheManager>()
                .AddTransient<PurchasingDocumentExpeditionFacade>()
                .AddTransient<IBankExpenditureNoteFacade, BankExpenditureNoteFacade>()
                .AddTransient<IBankDocumentNumberGenerator, BankDocumentNumberGenerator>()
                .AddTransient<PurchasingDocumentExpeditionReportFacade>()
                .AddTransient<IPPHBankExpenditureNoteFacade, PPHBankExpenditureNoteFacade>()
                .AddTransient<IPPHBankExpenditureNoteReportFacade, PPHBankExpenditureNoteReportFacade>()
                .AddTransient<IUnitPaymentOrderPaidStatusReportFacade, UnitPaymentOrderPaidStatusReportFacade>()
                .AddTransient<IUnitPaymentOrderUnpaidReportFacade, UnitPaymentOrderUnpaidReportFacade>()
                .AddTransient<UnitPaymentOrderNotVerifiedReportFacade>()
                .AddTransient<PurchaseRequestFacade>()
                .AddTransient<DeliveryOrderFacade>()
                .AddTransient<IDeliveryOrderFacade, DeliveryOrderFacade>()
                .AddTransient<InternalPurchaseOrderFacade>()
                .AddTransient<ExternalPurchaseOrderFacade>()
                .AddTransient<MonitoringPriceFacade>()
                .AddTransient<IUnitReceiptNoteFacade, UnitReceiptNoteFacade>()
                .AddTransient<TotalPurchaseFacade>()
                .AddTransient<PurchaseRequestGenerateDataFacade>()
                .AddTransient<ExternalPurchaseOrderGenerateDataFacade>()
                .AddTransient<UnitReceiptNoteGenerateDataFacade>()
                .AddTransient<IUnitPaymentOrderFacade, UnitPaymentOrderFacade>()
                .AddTransient<IUnitPaymentQuantityCorrectionNoteFacade, UnitPaymentQuantityCorrectionNoteFacade>()
                .AddTransient<IUnitPaymentPriceCorrectionNoteFacade, UnitPaymentPriceCorrectionNoteFacade>()
                .AddTransient<PurchaseOrderMonitoringAllFacade>()
                .AddTransient<IGarmentPurchaseRequestFacade, GarmentPurchaseRequestFacade>()
                .AddTransient<IGarmentInternalPurchaseOrderFacade, GarmentInternalPurchaseOrderFacade>()
                .AddTransient<IGarmentTotalPurchaseOrderFacade, TotalGarmentPurchaseFacade>()
                .AddTransient<IGarmentInvoice, GarmentInvoiceFacade>()
                .AddTransient<IGarmentInternNoteFacade, GarmentInternNoteFacades>()
                .AddTransient<IGarmentExternalPurchaseOrderFacade, GarmentExternalPurchaseOrderFacade>()
                .AddTransient<IGarmentDeliveryOrderFacade, GarmentDeliveryOrderFacade>()
                .AddTransient<IPurchasingDispositionFacade, PurchasingDispositionFacade>()
                .AddTransient<IGarmentCorrectionNotePriceFacade, GarmentCorrectionNotePriceFacade>()
                .AddTransient<IGarmentCorrectionNoteQuantityFacade, GarmentCorrectionNoteQuantityFacade>()
                .AddTransient<IGarmentBeacukaiFacade, GarmentBeacukaiFacade>()
                .AddTransient<IPurchasingDispositionFacade, PurchasingDispositionFacade>()
                .AddTransient<IGarmentCorrectionNotePriceFacade, GarmentCorrectionNotePriceFacade>()
                .AddTransient<IGarmentUnitReceiptNoteFacade, GarmentUnitReceiptNoteFacade>()
                .AddTransient<IMonitoringUnitReceiptAllFacade, MonitoringUnitReceiptAllFacade>()
                .AddTransient<IGarmentUnitDeliveryOrderFacade, GarmentUnitDeliveryOrderFacade>()
                .AddTransient<IGarmentUnitExpenditureNoteFacade, GarmentUnitExpenditureNoteFacade>()
                .AddTransient<IGarmentReturnCorrectionNoteFacade, GarmentReturnCorrectionNoteFacade>()
                .AddTransient<IGarmentUnitDeliveryOrderReturFacade, GarmentUnitDeliveryOrderReturFacade>()
                .AddTransient<IMonitoringCentralBillReceptionFacade, MonitoringCentralBillReceptionFacade>()
                .AddTransient<IMonitoringCentralBillExpenditureFacade, MonitoringCentralBillExpenditureFacade>()
                .AddTransient<IMonitoringCorrectionNoteReceptionFacade, MonitoringCorrectionNoteReceptionFacade>()
                .AddTransient<IMonitoringCorrectionNoteExpenditureFacade, MonitoringCorrectionNoteExpenditureFacade>()
                .AddTransient<IGarmentDailyPurchasingReportFacade, GarmentDailyPurchasingReportFacade>()
                .AddTransient<IGarmentReceiptCorrectionFacade, GarmentReceiptCorrectionFacade>()
                .AddTransient<IGarmentPOMasterDistributionFacade, GarmentPOMasterDistributionFacade>()
                .AddTransient<IMonitoringROJobOrderFacade, MonitoringROJobOrderFacade>()
                .AddTransient<IMonitoringROMasterFacade, MonitoringROMasterFacade>()
                .AddTransient<IBudgetMasterSampleDisplayFacade, BudgetMasterSampleDisplayFacade>()
                .AddTransient<IUnitPaymentOrderExpeditionReportService, UnitPaymentOrderExpeditionReportService>()
                .AddTransient<ILocalPurchasingBookReportFacade, LocalPurchasingBookReportFacade>()
                .AddTransient<IImportPurchasingBookReportFacade, ImportPurchasingBookReportFacade>()
                .AddTransient<ICurrencyProvider, CurrencyProvider>()
                .AddTransient<IPurchaseMonitoringService, PurchaseMonitoringService>()
                .AddTransient<IGarmentPurchasingBookReportFacade, GarmentPurchasingBookReportFacade>();
                
        }

        private void RegisterServices(IServiceCollection services, bool isTest)
        {
            services
                .AddScoped<IdentityService>()
                .AddScoped<ValidateService>()
                .AddScoped<IValidateService, ValidateService>();

            if (isTest == false)
            {
                services.AddScoped<IHttpClientService, HttpClientService>();
            }
        }

        private void RegisterSerializationProvider()
        {
            BsonSerializer.RegisterSerializationProvider(new SerializationProvider());
        }

        private void RegisterClassMap()
        {
            ClassMap<UnitReceiptNoteViewModel>.Register();
            ClassMap<UnitReceiptNoteItemViewModel>.Register();
            ClassMap<UnitViewModel>.Register();
            ClassMap<DivisionViewModel>.Register();
            ClassMap<CategoryViewModel>.Register();
            ClassMap<ProductViewModel>.Register();
            ClassMap<UomViewModel>.Register();
            ClassMap<PurchaseOrderViewModel>.Register();
            ClassMap<SupplierViewModel>.Register();
            ClassMap<UnitPaymentCorrectionNoteViewModel>.Register();
        }

        #endregion Register

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString(Constant.DEFAULT_CONNECTION) ?? Configuration[Constant.DEFAULT_CONNECTION];
            string env = Configuration.GetValue<string>(Constant.ASPNETCORE_ENVIRONMENT);
            APIEndpoint.ConnectionString = Configuration.GetConnectionString("DefaultConnection") ?? Configuration["DefaultConnection"];

            /* Register */
            //services.AddDbContext<PurchasingDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<PurchasingDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(1000 * 60 * 20)));
            RegisterEndpoints();
            RegisterFacades(services);
            RegisterServices(services, env.Equals("Test"));
            services.AddAutoMapper(typeof(BaseAutoMapperProfile));
            services.AddMemoryCache();

            RegisterSerializationProvider();
            RegisterClassMap();
            MongoDbContext.connectionString = Configuration.GetConnectionString(Constant.MONGODB_CONNECTION) ?? Configuration[Constant.MONGODB_CONNECTION];


            /* Versioning */
            services.AddApiVersioning(options => { options.DefaultApiVersion = new ApiVersion(1, 0); });

            /* Authentication */
            string Secret = Configuration.GetValue<string>(Constant.SECRET) ?? Configuration[Constant.SECRET];
            SymmetricSecurityKey Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Secret));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = Key
                    };
                });

            /* CORS */
            services.AddCors(options => options.AddPolicy(PURCHASING_POLICITY, builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders(EXPOSED_HEADERS);
            }));

            /* API */
            services
               .AddMvcCore()
               .AddApiExplorer()
               .AddAuthorization()
               .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
               .AddJsonFormatters();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Spinning API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme { In = "header", Description = "Please enter JWT with Bearer into field", Name = "Authorization", Type = "apiKey" });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() },
                });

                c.CustomSchemaIds(i => i.FullName);
            });

            // App Insight Telemetry
            services.AddApplicationInsightsTelemetry();
            services.AddApplicationInsightRequestBodyLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /* Update Database */
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                PurchasingDbContext context = serviceScope.ServiceProvider.GetService<PurchasingDbContext>();
                context.Database.SetCommandTimeout(10 * 60 * 1000);
                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    context.Database.Migrate();
                }
            }

            // App Insight Telemetry
            app.UseApplicationInsightRequestBodyLogging();
            app.UseApplicationInsightResponseBodyLogging();

            app.UseAuthentication();
            app.UseCors(PURCHASING_POLICITY);
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });

            

            JobManager.Initialize(new MasterRegistry(app.ApplicationServices));
        }
    }
}
