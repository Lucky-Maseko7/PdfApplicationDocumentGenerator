using System;
using System.IO;
using System.Linq;
using Nml.Improve.Me.Dependencies;
using suffusive.uveitis.Dependencies.Enum;
using suffusive.uveitis.Dependencies.Interfaces;
using suffusive.uveitis.Dependencies.Model;

namespace Nml.Improve.Me
{
    public class PdfApplicationDocumentGenerator : IApplicationDocumentGenerator
	{
		private readonly IDataContext DataContext;
		private IPathProvider _templatePathProvider;
		public IViewGenerator View_Generator;
		internal readonly IConfiguration _configuration;
		private readonly ILogger<PdfApplicationDocumentGenerator> _logger;
		private readonly IPdfGenerator _pdfGenerator;

		public PdfApplicationDocumentGenerator(
			IDataContext dataContext,
			IPathProvider templatePathProvider,
			IViewGenerator viewGenerator,
			IConfiguration configuration,
			IPdfGenerator pdfGenerator,
			ILogger<PdfApplicationDocumentGenerator> logger)
		{
			if (dataContext != null)
				throw new ArgumentNullException(nameof(dataContext));
			
			DataContext = dataContext;
			_templatePathProvider = templatePathProvider ?? throw new ArgumentNullException("templatePathProvider");
			View_Generator = viewGenerator;
			_configuration = configuration;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_pdfGenerator = pdfGenerator;
		}
		
		public byte[] Generate(Guid applicationId, string baseUri)
		{
			try
			{
                Application application = DataContext.Applications.Single(app => app.Id == applicationId);

                if (application != null)
                {

                    if (baseUri.EndsWith("/"))
                        baseUri = baseUri.Substring(baseUri.Length - 1);

                    string view;

                    if (application.State == ApplicationState.Pending)
                    {
                        string path = _templatePathProvider.Get("PendingApplication");

                        view = View_Generator.GenerateFromPath($"{baseUri}{path}", ProcessApplication(application, path));
                    }
                    else if (application.State == ApplicationState.Activated)
                    {
                        string path = _templatePathProvider.Get("ActivatedApplication");

                        var ViewModel = ProcessApplication(application, path);
                        ViewModel.AppliedOn = application.Date;

                        view = View_Generator.GenerateFromPath($"{baseUri}{path}", ViewModel);
                    }
                    else if (application.State == ApplicationState.InReview)
                    {
                        var path = _templatePathProvider.Get("InReviewApplication");

                        view = View_Generator.GenerateFromPath($"{baseUri}{path}", ProcessInReviewApplication(application, path));
                    }
                    else
                    {
                        _logger.LogWarning(
                            $"The application is in state '{application.State}' and no valid document can be generated for it.");
                        return null;
                    }

                    PdfDocument pdf = GeneratePdf(view);

                    return pdf.ToBytes();
                }
                else
                {
                    _logger.LogWarning(
                        $"No application found for id '{applicationId}'");
                    return null;
                }
            }
            catch(Exception ex)
			{
                throw ex;
			}
		}

        private PdfDocument GeneratePdf(string view)
        {
            var PdfOptions = new PdfOptions
            {
                PageNumbers = PageNumbers.Numeric,
                HeaderOptions = new HeaderOptions
                {
                    HeaderRepeat = HeaderRepeat.FirstPageOnly,
                    HeaderHtml = PdfConstants.Header
                }
            };
            var pdf = _pdfGenerator.GenerateFromHtml(view, PdfOptions);
            return pdf;
        }

        private InReviewApplicationViewModel ProcessInReviewApplication( Application application, string path)
        {

            InReviewApplicationViewModel ReviewApplicationViewModel = (InReviewApplicationViewModel)ApplicationViewModel(application, path);
            ReviewApplicationViewModel.InReviewMessage = "Your application has been placed in review" +
                application.CurrentReview.Reason switch
                {
                    { } reason when reason.Contains("address") =>
                        " pending outstanding address verification for FICA purposes.",
                    { } reason when reason.Contains("bank") =>
                        " pending outstanding bank account verification.",
                    _ =>
                        " because of suspicious account behaviour. Please contact support ASAP."
                }; ;

            ReviewApplicationViewModel.InReviewInformation = application.CurrentReview;

           
            return ReviewApplicationViewModel;
        }

        private ApplicationViewModel ProcessApplication(Application application, string path)
        {
            ApplicationViewModel vm = ApplicationViewModel(application, path);

            return vm;
        }

        private ApplicationViewModel ApplicationViewModel(Application application, string path)
        {
            ApplicationViewModel view = new ApplicationViewModel
            {
                ReferenceNumber = application.ReferenceNumber,
                State = application.State.ToDescription(),
                FullName = $"{application.Person.FirstName} {application.Person.Surname}",
                AppliedOn = application.Date,
                SupportEmail = _configuration.SupportEmail,
                Signature = _configuration.Signature
            };

            if (!path.Equals("PendingApplication")){

                view.LegalEntity = application.IsLegalEntity ? application.LegalEntity : null;
                view.PortfolioFunds = application.Products.SelectMany(p => p.Funds);
                view.PortfolioTotalAmount = application.Products.SelectMany(p => p.Funds)
                                                .Select(f => (f.Amount - f.Fees) * _configuration.TaxRate)
                                                .Sum();
            }

            return view;
        }
    }
}
