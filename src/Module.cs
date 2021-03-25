using Autofac;
using restlessmedia.Module.Email.Configuration;
using restlessmedia.Module.Email.Data;
using SendGrid;

namespace restlessmedia.Module.Email
{
  public class Module : IModule
  {
    public void RegisterComponents(ContainerBuilder containerBuilder)
    {
      containerBuilder.RegisterType<EmailService>().As<IEmailService>().SingleInstance();
      containerBuilder.RegisterType<EmailContext>().As<IEmailContext>().SingleInstance();
      containerBuilder.RegisterType<EmailQueueDataProvider>().As<IEmailQueueDataProvider>().SingleInstance();
      containerBuilder.RegisterSettings<IEmailSettings>("restlessmedia/email", required: true);
      containerBuilder.Register(context =>
      {
        IEmailSettings settings = context.Resolve<IEmailSettings>();
        return new SendGridClient(settings.ApiKey);
      })
        .As<ISendGridClient>()
        .SingleInstance();
    }
  }
}