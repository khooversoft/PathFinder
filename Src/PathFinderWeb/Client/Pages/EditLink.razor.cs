using Microsoft.AspNetCore.Components;
using PathFinder.sdk.Records;
using PathFinderWeb.Client.Application;
using PathFinderWeb.Client.Application.Menu;
using PathFinderWeb.Client.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Pages
{
    public partial class EditLink : ComponentBase
    {
        [Inject]
        public LinkService LinkService { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public string? Id { get; set; }

        public MenuCollection? MenuCollection { get; set; }

        public FormDataDetail FormData { get; set; } = new FormDataDetail();

        public string? ErrorMessage { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            BuildMenu();
            await LoadData();

            base.OnParametersSet();
        }

        private void BuildMenu()
        {
            (string Text, string IconName) = Id.IsEmpty() ? ("Create", "oi-plus") : ("Save", "oi-file");

            MenuCollection = new MenuCollection()
            {
                new MenuButton(Text, async () => await Save(), IconName, true),
                new MenuDivider(),
                new MenuItem("Cancel", NavigationHelper.LinkPage(), "oi-x", true),
            };
        }

        public async Task Save()
        {
            if (!Id.IsEmpty()) return;

            FormData.VerifyNotNull(nameof(FormData));

            var tests = new Func<string?>[]
            {
                () => FormData.Id.IsEmpty() ? "ID is required" : null,
                () => FormData.RedirectUrl.IsEmpty() ? "Url Redirect is required" : null,
            };

            tests
                .Select(x => x())
                .FirstOrDefault(x => x != null)
                ?.Action(x =>
                {
                    ErrorMessage = x;
                    StateHasChanged();
                    return;
                });

            try
            {
                var linkRecord = new LinkRecord
                {
                    Id = FormData.Id!,
                    RedirectUrl = FormData.RedirectUrl!,
                };

                await LinkService.Set(linkRecord);

                NavigationManager.NavigateTo(NavigationHelper.LinkPage());
            }
            catch
            {
                ErrorMessage = $"Failed to save Link Id={FormData.Id}";
                StateHasChanged();
                return;
            }
        }

        private async Task LoadData()
        {
            if (Id.IsEmpty()) return;

            try
            {
                LinkRecord result = await LinkService.Get(Id!);

                FormData = new FormDataDetail
                {
                    Id = result.Id,
                    RedirectUrl = result.RedirectUrl,
                };

            }
            catch
            {
                ErrorMessage = $"Failed to load Link Id={Id}";
            }

            StateHasChanged();
        }

        public class FormDataDetail
        {
            [Required]
            public string? Id { get; set; }

            [Required]
            [Url]
            public string? RedirectUrl { get; set; }
        }

    }
}
