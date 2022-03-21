using YourCompany.Portal.Services;

using MudBlazor;


namespace YourCompany.Portal.Services;

public class NotificationService : INotificationService
{
    private readonly ISnackbar snackbar;

    public NotificationService(ISnackbar snackbar)
    {
        this.snackbar = snackbar;
    }

    public void ShowNotification(string title, string body)
    {
        snackbar.Add(body);
    }
}