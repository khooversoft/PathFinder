using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
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
    public partial class EditLink : ComponentBase, IDisposable
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

        private EditContext EditContext { get; set; } = null!;

        private bool IsFormValid { get; set; } = false;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(FormData);
            EditContext.OnFieldChanged += FieldChange;

            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            BuildMenu();
            await LoadData();
        }

        public void Dispose()
        {
            if (EditContext != null)
            {
                EditContext.OnFieldChanged -= FieldChange;
            }
        }

        private void BuildMenu()
        {
            (string Text, string IconName) = Id.IsEmpty() ? ("Create", "oi-plus") : ("Save", "oi-file");

            MenuCollection = new MenuCollection()
            {
                new MenuButton(Text, async () => await Save(), IconName, IsFormValid),
                new MenuDivider(),
                new MenuItem("Cancel", NavigationHelper.LinkPage(), "oi-x", true),
            };
        }

        private async Task Save()
        {
            if (!Id.IsEmpty()) return;

            FormData.VerifyNotNull(nameof(FormData));

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

        private void FieldChange(object sender, FieldChangedEventArgs e)
        {
            Console.WriteLine($"{nameof(FieldChange)}");
            IsFormValid = !FormData.Id.IsEmpty() && !FormData.RedirectUrl.IsEmpty();
            StateHasChanged();
        }

        public class FormDataDetail
        {
            public string? Id { get; set; }

            public string? RedirectUrl { get; set; }
        }

    }
}
