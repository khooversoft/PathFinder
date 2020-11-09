using Microsoft.AspNetCore.Components;
using PathFinder.sdk.Models;
using PathFinder.sdk.Records;
using PathFinderWeb.Client.Application;
using PathFinderWeb.Client.Application.Menu;
using PathFinderWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Toolbox.Services;
using Toolbox.Tools;

namespace PathFinderWeb.Client.Pages
{
    public partial class Link : ComponentBase
    {
        [Inject]
        public LinkService LinkService { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        public RunContext Context { get; set; } = new RunContext();

        public MenuCollection MenuCollection { get; set; } = null!;

        protected override void OnParametersSet()
        {
            MenuCollection = new MenuCollection()
            {
                new MenuItem("Create", NavigationHelper.NewLinkPage(), IconHelper.Create, true),
                new MenuDivider(),
                new MenuButton("Refresh", async () => await GetLinks(), IconHelper.Reload, true),
            };

            base.OnParametersSet();
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
            try
            {
                Context.SetStartup();
                StateHasChanged();

                IReadOnlyList<LinkRecord> result = await LinkService.List();
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

        public class RunContext
        {
            public RunState RunState { get; private set; } = RunState.Startup;

            public int? Count { get; private set; }

            public IReadOnlyList<LinkRecord>? Records { get; private set; }

            public string? ErrorMessage { get; private set; }

            public void SetStartup() => Clear();

            public void SetMessages(int count, IEnumerable<LinkRecord> records)
            {
                Clear();
                RunState = RunState.Result;
                Count = count;
                Records = records.ToList();
            }

            public void SetError(string errorMessage)
            {
                Clear();
                RunState = RunState.Error;
                ErrorMessage = errorMessage;
            }

            private void Clear()
            {
                RunState = RunState.Startup;
                Count = null;
                Records = null;
                ErrorMessage = null;
            }
        }
    }
}
