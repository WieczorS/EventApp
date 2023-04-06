using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace WebApp;

public class CustomMessageHandler : DelegatingHandler
{
    readonly NavigationManager _navigationManager;
    private ILocalStorageService _localStorage;

    public CustomMessageHandler(ILocalStorageService localStorage,
        NavigationManager navigationManager)
    {
        _localStorage = localStorage;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = string.Empty;
        try
        {
            token = await _localStorage.GetItemAsStringAsync(Constants.TOKEN_NAME);
        }
        catch (Exception ex)
        {
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("\"", ""));
        
        var response = await base.SendAsync(request, cancellationToken);

        if (request.RequestUri.ToString().Contains("login") == false &&
            response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _localStorage.RemoveItemAsync("token");
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}