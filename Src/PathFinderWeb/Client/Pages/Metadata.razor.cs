using Microsoft.AspNetCore.Components;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinderWeb.Client.Application;
using PathFinderWeb.Client.Application.Menu;
using PathFinderWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Toolbox.Extensions;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Pages
{
    public partial class Metadata : ComponentBase
    {
        private readonly DelayAction _delayAction = new DelayAction(TimeSpan.FromMilliseconds(800));
        private readonly ActionBlock<Action> _actionQueue;

        public Metadata()
        {
            _actionQueue = new ActionBlock<Action>(async x =>
            {
                x();
                Context.Clear();
                await GetLinks();
            });
        }

        [Inject]
        StateCacheService StateCacheService { get; set; } = null!;

        [Inject]
        public MetadataService MetadataService { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public RunContext<MetadataRecord> Context { get; set; } = null!;

        public MenuCollection MenuCollection { get; set; } = null!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Context = StateCacheService.GetOrCreate(() => new RunContext<MetadataRecord>());

            MenuCollection = new MenuCollection()
            {
                new MenuItem("Create", NavigationHelper.Metadata.NewMetadataPage(), IconHelper.Create, true),
                new MenuDivider(),
                new MenuButton("Refresh", async () => await GetLinks(), IconHelper.Reload, true),
                new MenuButton("Clear search", ResetSearch, IconHelper.Reset, true),
            };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await GetLinks();
            }
        }

        protected async Task GetLinks()
        {
            QueryParameters queryParameters = new QueryParameters
            {
                Id = Context.SearchByIdUrl,
                Tag = Context.SearchByTag,
                Owner = Context.SearchByOwner,
            };

            try
            {
                Context.SetStartup();
                StateHasChanged();

                if ((Context.Records?.Count ?? 0) == 0)
                {
                    IReadOnlyList<MetadataRecord> result = await MetadataService.List(queryParameters);
                    Context.SetMessages(result);
                }
            }
            catch
            {
                Context.SetError($"Failed to get any link records from server");
            }
            finally
            {
                StateHasChanged();
            }
        }

        private void OnRowSelect(MetadataRecord metadataRecord)
        {
            metadataRecord.VerifyNotNull(nameof(metadataRecord));

            NavigationManager.NavigateTo(NavigationHelper.Metadata.EditMetadataPage(metadataRecord.Id));
        }

        private async Task ResetSearch()
        {
            Context.SearchByIdUrl = null;
            Context.SearchByOwner = null;
            Context.SearchByTag = null;

            await GetLinks();
        }

        private void OnSearchChange(string value) => _delayAction.Post(() => _actionQueue.Post(() => Context.SearchByIdUrl = value));

        private void OnSearchTagChange(string value) => _delayAction.Post(() => _actionQueue.Post(() => Context.SearchByTag = value));

        private void OnSearchOwnerChange(string value) => _delayAction.Post(() => _actionQueue.Post(() => Context.SearchByOwner = value));
    }
}
