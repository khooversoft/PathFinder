using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolbox.Extensions;

namespace PathFinder.sdk.Models
{
    public static class QueryParametersExtensions
    {
        public static QueryParameters WithIndex(this QueryParameters queryParameters, int index) => new QueryParameters(queryParameters)
        {
            Index = index,
        };

        public static string ToQuery(this QueryParameters queryParameters)
        {
            string query = new string?[]
            {
                (nameof(queryParameters.Index), queryParameters.Index).FormatQuery(),
                (nameof(queryParameters.Count), queryParameters.Count).FormatQuery(),

                (nameof(queryParameters.Id), queryParameters.Id).FormatQuery(),
                (nameof(queryParameters.RedirectUrl), queryParameters.RedirectUrl).FormatQuery(),
                (nameof(queryParameters.Owner), queryParameters.Owner).FormatQuery(),
                (nameof(queryParameters.Tag), queryParameters.Tag).FormatQuery(),
            }
            .Where(x => x != null)
            .Func(x => string.Join("&", x));

            return query;
        }

        public static string? ToSqlOffset(this QueryParameters queryParameters) => $"OFFSET {queryParameters.Index}";

        public static string? ToSqlLimit(this QueryParameters queryParameters) => $"LIMIT {queryParameters.Count}";

        public static string? ToSqlIdSearch(this QueryParameters queryParameters) => ConstructContains("r.id", queryParameters.Id);

        public static string? ToSqlRedirectUrlSearch(this QueryParameters queryParameters) => ConstructContains("r.RedirectUrl", queryParameters.RedirectUrl);

        public static string? ToSqlOwnerSearch(this QueryParameters queryParameters) => ConstructContains("r.Owner", queryParameters.Owner);

        private static string FormatQuery(this (string name, int value) subject) => $"{subject.name}={subject.value}";

        private static string? FormatQuery(this (string name, string? value) subject) => !subject.value.IsEmpty() ? $"{subject.name}={subject.value}" : null;

        private static string? ConstructContains(string reference, string? value) =>
            !value.IsEmpty() ? $"CONTAINS({reference}, \"{value}\", true)" : null;
    }
}