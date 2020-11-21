using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinderWeb.Client.Application;
using PathFinderWeb.Client.Application.Menu;
using PathFinderWeb.Client.Components;
using PathFinderWeb.Client.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Pages
{
    public partial class EditMetadata : ComponentBase, IDisposable
    {
        [Inject]
        public MetadataService MetadataService { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Parameter]
        public string? Id { get; set; }

        private MenuCollection? MenuCollection { get; set; }

        private MetadataRecord FormData { get; set; } = new MetadataRecord();

        private MetadataRecord? CurrentFormData { get; set; }

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
                new MenuItem("Cancel", NavigationHelper.Metadata.MetadataPage(), IconHelper.Cancel, true),
            };
        }

        private async Task Save()
        {
            FormData.VerifyNotNull(nameof(FormData));
            ErrorMessage = null;

            try
            {
                await MetadataService.Set(new MetadataRecord(FormData));

                NavigationManager.NavigateTo(NavigationHelper.Metadata.MetadataPage());
            }
            catch
            {
                ErrorMessage = $"Failed to save Metadata Id={FormData.Id}";
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

            await MetadataService.Delete(Id!);

            NavigationManager.NavigateTo(NavigationHelper.Link.LinkPage());
        }

        private async Task LoadData()
        {
            ErrorMessage = null;
            if (Id.IsEmpty()) return;

            try
            {
                FormData = (await MetadataService.Get(Id!)) ?? throw new ArgumentException();
                CurrentFormData = new MetadataRecord(FormData);

                FormData.Tags.Add(new KeyValue(string.Empty, string.Empty));
            }
            catch
            {
                ErrorMessage = $"Failed to load Link Id={Id}";
            }

            StateHasChanged();
        }

        private void OnChangeId(string value) => this.Action(x => FormData.Id = value).SetCanSave();

        private void OnChangeOwner(string value) => this.Action(x => FormData.Id = value).SetCanSave();

        private void SetCanSave()
        {
            CanSave = !Id.IsEmpty() &&
                FormData != CurrentFormData;

            BuildMenu();
            StateHasChanged();
        }

        private void VerifyExtraTag()
        {
            var tagList = FormData.Tags.ToList();

            if (FormData.Tags.Any(x => x.Key.IsEmpty() || x.Value.IsEmpty())) return;

            FormData.Tags.Add(new KeyValue(string.Empty, string.Empty));

            SetCanSave();
        }

        private void VerifyExtraProperty()
        {
            var tagList = FormData.Properties.ToList();

            if (FormData.Properties.Any(x => x.Key.IsEmpty() || x.Value.IsEmpty())) return;

            FormData.Properties.Add(new KeyValue(string.Empty, string.Empty));

            SetCanSave();
        }
    }
}