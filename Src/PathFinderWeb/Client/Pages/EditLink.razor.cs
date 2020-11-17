using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using PathFinder.sdk.Records;
using PathFinder.sdk.Services.RecordAbstract;
using PathFinderWeb.Client.Application;
using PathFinderWeb.Client.Application.Menu;
using PathFinderWeb.Client.Components;
using PathFinderWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml.Schema;
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

        private MenuCollection? MenuCollection { get; set; }

        private LinkRecord FormData { get; set; } = new LinkRecord();

        private LinkRecord? CurrentFormData { get; set; }

        private string? ErrorMessage { get; set; }

        private EditContext EditContext { get; set; } = null!;

        private Modal Modal { get; set; } = null!;

        private bool CanSave { get; set; } = false;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(FormData);

            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            BuildMenu();
            await LoadData();
        }

        public void Dispose()
        {
        }

        private void BuildMenu()
        {
            (string Text, Icon Icon) = Id.IsEmpty() ? ("Create", IconHelper.Add) : ("Save", IconHelper.Save);

            MenuCollection = new MenuCollection()
            {
                new MenuButton(Text, async () => await Save(), Icon, CanSave),
                new MenuDivider(),
                !Id.IsEmpty() ? new MenuButton("Delete", async () => await ShowDeleteDialog(), IconHelper.Delete, true) : null,
                !Id.IsEmpty() ? new MenuDivider() : null,
                new MenuItem("Cancel", NavigationHelper.LinkPage(), IconHelper.Cancel, true),
            };
        }

        private async Task Save()
        {
            FormData.VerifyNotNull(nameof(FormData));
            ErrorMessage = null;

            try
            {
                await LinkService.Set(new LinkRecord(FormData));

                NavigationManager.NavigateTo(NavigationHelper.LinkPage());
            }
            catch
            {
                ErrorMessage = $"Failed to save Link Id={FormData.Id}";
                StateHasChanged();
                return;
            }
        }

        private Task ShowDeleteDialog()
        {
            Modal.Open();
            return Task.CompletedTask;
        }

        private async Task Delete()
        {
            if (Id.IsEmpty()) return;

            await LinkService.Delete(Id!);

            NavigationManager.NavigateTo(NavigationHelper.LinkPage());
        }

        private async Task LoadData()
        {
            ErrorMessage = null;
            if (Id.IsEmpty()) return;

            try
            {
                FormData = await LinkService.Get(Id!);
                CurrentFormData = new LinkRecord(FormData);

                FormData.Tags.Add(new Tag(string.Empty, string.Empty));
            }
            catch
            {
                ErrorMessage = $"Failed to load Link Id={Id}";
            }

            StateHasChanged();
        }

        private void OnChangeId(string value) => this.Action(x => FormData.Id = value).SetCanSave();

        private void OnChangeRedirectUrl(string value) => this.Action(x => FormData.Id = value).SetCanSave();

        private void OnChangeOwner(string value) => this.Action(x => FormData.Id = value).SetCanSave();

        private void SetCanSave()
        {
            CanSave = !Id.IsEmpty() &&
                !FormData.RedirectUrl.IsEmpty() &&
                FormData != CurrentFormData;

            BuildMenu();
            StateHasChanged();
        }

        private void VerifyExtra()
        {
            var tagList = FormData.Tags.ToList();

            if (FormData.Tags.Any(x => x.Key.IsEmpty() || x.Value.IsEmpty())) return;

            FormData.Tags.Add(new Tag(string.Empty, string.Empty));

            SetCanSave();
        }
    }
}
