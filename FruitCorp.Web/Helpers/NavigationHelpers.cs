using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using System.Linq;
using System.Web.Mvc;

namespace FruitCorp.Web.Helpers
{
    public static class NavigationHelpers
    {
        public static void RenderMainNavigation(
            this HtmlHelper html,
            PageReference rootLink = null,
            ContentReference contentLink = null,
            bool includeRoot = true,
            IContentLoader contentLoader = null)
        {
            contentLink = contentLink ?? 
                html.ViewContext.RequestContext.GetContentLink();
            rootLink = rootLink ??
                ContentReference.StartPage;

            var writer = html.ViewContext.Writer;

            //Top level elements
            writer.WriteLine("<nav class=\"navbar navbar-inverse\">");
            writer.WriteLine("<ul class=\"nav navbar-nav\">");

            if (includeRoot)
            {
                //Link to the root page
                if (rootLink.CompareToIgnoreWorkID(contentLink))
                {
                    writer.WriteLine("<li class=\"active\">");
                }
                else
                {
                    writer.WriteLine("<li>");
                }
                writer.WriteLine(html.PageLink(rootLink).ToHtmlString());
                writer.WriteLine("</li>");
            }

            //Retrieve and filter the root page's children
            contentLoader = contentLoader ??
                ServiceLocator.Current.GetInstance<IContentLoader>();
            var topLevelPages = contentLoader
                .GetChildren<PageData>(rootLink);
            topLevelPages = FilterForVisitor.Filter(topLevelPages)
                .OfType<PageData>()
                .Where(x => x.VisibleInMenu);

            var currentBranch = contentLoader.GetAncestors(contentLink)
                .Select(x => x.ContentLink)
                .ToList();
            currentBranch.Add(contentLink);

            //Link to the root page's children
            foreach (var topLevelPage in topLevelPages)
            {
                if (currentBranch.Any(x => x.CompareToIgnoreWorkID(topLevelPage.ContentLink)))
                {
                    writer.WriteLine("<li class=\"active\">");
                }
                else
                {
                    writer.WriteLine("<li>");
                }
                writer.WriteLine(html.PageLink(topLevelPage).ToHtmlString());
                writer.WriteLine("</li>");
            }

            //Close top level elements
            writer.WriteLine("</ul>");
            writer.WriteLine("</nav>");
        }
    }
}