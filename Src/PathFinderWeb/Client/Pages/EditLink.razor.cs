using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using PathFinder.sdk.Records;
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
            if (Id.IsEmpty()) return;

            try
            {
                FormData = await LinkService.Get(Id!);
                CurrentFormData = new LinkRecord(FormData);
            }
            catch
            {
                ErrorMessage = $"Failed to load Link Id={Id}";
            }

            StateHasChanged();
        }

        private void FieldChange(object sender, FieldChangedEventArgs e)
        {
            CanSave = !FormData.Id.IsEmpty() && !FormData.RedirectUrl.IsEmpty() && (Id.IsEmpty() || CurrentFormData != FormData);
            BuildMenu();
            StateHasChanged();
        }

        //public class FormDataDetail
        //{
        //    public string? Id { get; set; }

        //    public string? RedirectUrl { get; set; }

        //    public bool Enabled { get; set; }

        //    public FormDataDetail Clone()
        //    {
        //        return new FormDataDetail
        //        {
        //            Id = Id,
        //            RedirectUrl = RedirectUrl,
        //            Enabled = Enabled,
        //        };
        //    }

        //    public override bool Equals(object? obj)
        //    {
        //        return obj is FormDataDetail detail &&
        //               Id == detail.Id &&
        //               RedirectUrl == detail.RedirectUrl &&
        //               Enabled == detail.Enabled;
        //    }

        //    public override int GetHashCode() => HashCode.Combine(Id, RedirectUrl, Enabled);

        //    public static bool operator ==(FormDataDetail? left, FormDataDetail? right) => EqualityComparer<FormDataDetail>.Default.Equals(left!, right!);

        //    public static bool operator !=(FormDataDetail? left, FormDataDetail? right) => !(left == right);
        //}
    }
}
