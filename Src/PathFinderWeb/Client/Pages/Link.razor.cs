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
    public partial class Link : ComponentBase
    {
        private readonly DelayAction _delayAction = new DelayAction(TimeSpan.FromMilliseconds(800));
        private readonly ActionBlock<Action> _actionQueue;

        public Link()
        {
            _actionQueue = new ActionBlock<Action>(async x =>
            {
                x();
                await GetLinks();
            });
        }

        [Inject]
        StateCacheService StateCacheService { get; set; } = null!;

        [Inject]
        public LinkService LinkService { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public RunContext<LinkRecord> Context { get; set; } = null!;

        public MenuCollection MenuCollection { get; set; } = null!;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Context = StateCacheService.GetOrCreate(() => new RunContext<LinkRecord>());

            MenuCollection = new MenuCollection()
            {
                new MenuItem("Create", NavigationHelper.NewLinkPage(), IconHelper.Create, true),
                new MenuDivider(),
                new MenuButton("Refresh", async () => await GetLinks(), IconHelper.Reload, true),
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
                RedirectUrl = Context.SearchByIdUrl,
                Tag = Context.SearchByTag,
                Owner = Context.SearchByOwner,
            };

            try
            {
                Context.SetStartup();
                StateHasChanged();

                IReadOnlyList<LinkRecord> result = await LinkService.List(queryParameters);

                Context.SetMessages(result.Count, result);
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

        private void OnRowSelect(LinkRecord linkRecord)
        {
            linkRecord.VerifyNotNull(nameof(linkRecord));

            NavigationManager.NavigateTo(NavigationHelper.EditLinkPage(linkRecord.Id));
        }

        private void OnSearchChange(string value) => _delayAction.Post(() => _actionQueue.Post(() => Context.SearchByIdUrl = value));

        private void OnSearchTagChange(string value) => _delayAction.Post(() => _actionQueue.Post(() => Context.SearchByTag = value));

        private void OnSearchOwnerChange(string value) => _delayAction.Post(() => _actionQueue.Post(() => Context.SearchByOwner = value));
    }
}
