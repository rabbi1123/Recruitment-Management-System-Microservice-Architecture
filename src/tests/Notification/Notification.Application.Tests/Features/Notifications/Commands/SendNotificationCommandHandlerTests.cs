using Common.Platform.Domain.Abstractions;
using Moq;
using Notification.Application.Common;
using Notification.Application.Features.Notifications.Commands;
using Notification.Application.Interfaces;
using NotificationRow = Notification.Domain.Entities.Notifications;
using Xunit;

namespace Notification.Application.Tests.Features.Notifications.Commands;

public class SendNotificationCommandHandlerTests
{
	private readonly Mock<INotificationRepository> _repository = new();
	private readonly Mock<IEmailTemplateService> _templates = new();
	private readonly Mock<IEmailService> _email = new();
	private readonly Mock<ISmsService> _sms = new();
	private readonly Mock<IPushService> _push = new();
	private readonly NotificationContent _content = new("Employee registered", "Employee registered", "Employee ada@example.com registered.");

	public SendNotificationCommandHandlerTests()
	{
		_templates
			.Setup(template => template.Render(It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>()))
			.Returns(_content);

		_email
			.Setup(service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new DeliveryResult("Smtp", "email-id"));

		_sms
			.Setup(service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new DeliveryResult("Log", "sms-id"));

		_push
			.Setup(service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new DeliveryResult("Log", "push-id"));

		_repository
			.Setup(repository => repository.ExistsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		_repository
			.Setup(repository => repository.AddAsync(It.IsAny<NotificationRow>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(1);
	}

	[Fact]
	public async Task Email_StoresRealAddress_AndSendsThroughSmtp()
	{
		var handler = new SendEmailNotificationCommandHandler(_repository.Object, _templates.Object, _email.Object);
		var eventId = Guid.NewGuid();

		var result = await handler.Handle(new SendEmailNotificationCommand
		{
			EventId = eventId,
			TemplateName = NotificationTemplateNames.EmployeeRegistered,
			Recipient = "ada@example.com",
			Tokens = new Dictionary<string, string> { ["Email"] = "ada@example.com" }
		}, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal(1, result.Data!.Id);

		_email.Verify(service => service.SendAsync(
			"ada@example.com",
			_content.Subject,
			_content.Body,
			It.IsAny<CancellationToken>()), Times.Once);

		_repository.Verify(repository => repository.AddAsync(
			It.Is<NotificationRow>(row =>
				row.EventId == eventId &&
				row.Channel == NotificationChannels.Email &&
				row.Recipient == "ada@example.com" &&
				row.Subject == _content.Subject &&
				row.Title == null &&
				row.Body == _content.Body &&
				row.Status == "SENT" &&
				row.Provider == "Smtp"),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Sms_StoresPlaceholderRecipient()
	{
		var handler = new SendSmsNotificationCommandHandler(_repository.Object, _templates.Object, _sms.Object);
		var recipient = NotificationRecipient.Resolve(
			NotificationChannels.Sms,
			NotificationTemplateNames.JobPublished,
			"42");

		var result = await handler.Handle(new SendSmsNotificationCommand
		{
			EventId = Guid.NewGuid(),
			TemplateName = NotificationTemplateNames.JobPublished,
			Recipient = recipient
		}, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal("JobPublished:42", recipient);

		_repository.Verify(repository => repository.AddAsync(
			It.Is<NotificationRow>(row =>
				row.Channel == NotificationChannels.Sms &&
				row.Recipient == "JobPublished:42" &&
				row.Subject == null &&
				row.Title == null &&
				row.Provider == "Log" &&
				row.Status == "SENT"),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Push_StoresTitleAndPlaceholderRecipient()
	{
		var handler = new SendPushNotificationCommandHandler(_repository.Object, _templates.Object, _push.Object);
		var recipient = NotificationRecipient.Resolve(
			NotificationChannels.Push,
			NotificationTemplateNames.EmployeeRegistered,
			"user-1",
			"ada@example.com");

		var result = await handler.Handle(new SendPushNotificationCommand
		{
			EventId = Guid.NewGuid(),
			TemplateName = NotificationTemplateNames.EmployeeRegistered,
			Recipient = recipient
		}, CancellationToken.None);

		Assert.True(result.IsSuccess);
		Assert.Equal("EmployeeRegistered:user-1", recipient);

		_push.Verify(service => service.SendAsync(
			recipient,
			_content.Title,
			_content.Body,
			It.IsAny<CancellationToken>()), Times.Once);

		_repository.Verify(repository => repository.AddAsync(
			It.Is<NotificationRow>(row =>
				row.Channel == NotificationChannels.Push &&
				row.Recipient == recipient &&
				row.Title == _content.Title &&
				row.Subject == null &&
				row.Provider == "Log"),
			It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Email_WhenAlreadyStored_DoesNotSendOrInsertAgain()
	{
		_repository
			.Setup(repository => repository.ExistsAsync(It.IsAny<Guid>(), NotificationChannels.Email, It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		var handler = new SendEmailNotificationCommandHandler(_repository.Object, _templates.Object, _email.Object);

		var result = await handler.Handle(new SendEmailNotificationCommand
		{
			EventId = Guid.NewGuid(),
			TemplateName = NotificationTemplateNames.EmployeeRegistered,
			Recipient = "ada@example.com"
		}, CancellationToken.None);

		Assert.True(result.IsSuccess);
		_email.Verify(
			service => service.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
			Times.Never);
		_repository.Verify(
			repository => repository.AddAsync(It.IsAny<NotificationRow>(), It.IsAny<CancellationToken>()),
			Times.Never);
	}

	[Fact]
	public void Recipient_UsesEmailOnlyForEmployeeRegisteredEmail()
	{
		Assert.Equal(
			"ada@example.com",
			NotificationRecipient.Resolve(NotificationChannels.Email, NotificationTemplateNames.EmployeeRegistered, "user-1", "ada@example.com"));

		Assert.Equal(
			"ada@example.com",
			NotificationRecipient.Resolve(NotificationChannels.Email, NotificationTemplateNames.EmailVerification, "user-1", "ada@example.com"));

		Assert.Equal(
			"JobPublished:42",
			NotificationRecipient.Resolve(NotificationChannels.Email, NotificationTemplateNames.JobPublished, "42", "ada@example.com"));
	}
}
