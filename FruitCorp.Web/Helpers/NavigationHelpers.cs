using EPiServer;
using EPiServer.Core;
using EPiServer.Filters;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.Html;
using EPiServer.Web.Routing;
using System.Linq;
using System.Web.Mvc;
using System;
using System.Collections.Generic;

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

        public static void RenderSubNavigation(
            this HtmlHelper html,
            ContentReference contentLink = null,
            IContentLoader contentLoader = null)
        {
            contentLink = contentLink ??
                html.ViewContext.RequestContext.GetContentLink();
            contentLoader = contentLoader ??
                ServiceLocator.Current.GetInstance<IContentLoader>();

            //Find all pages between the current and the start page, in top-down order.
            var path = contentLoader.GetAncestors(contentLink)
                .Reverse()
                .SkipWhile(x =>
                ContentReference.IsNullOrEmpty(x.ParentLink)
                || !x.ParentLink.CompareToIgnoreWorkID(ContentReference.StartPage))
                .OfType<PageData>()
                .Select(x => x.PageLink)
                .ToList();

            //Check if content is a page. If so, add it to the content tree path.
            var currentPage = contentLoader
                .Get<IContent>(contentLink) as PageData;
            if (currentPage != null)
            {
                path.Add(currentPage.PageLink);
            }

            var root = path.FirstOrDefault();
            if (root == null)
            {
                return;
            }

            RenderSubNavigationLevel(
                html,
                root,
                path,
                contentLoader);
        }

        private static void RenderSubNavigationLevel(
            HtmlHelper helper, 
            ContentReference levelRootLink, 
            IEnumerable<ContentReference> path, 
            IContentLoader contentLoader)
        {
            //Retrieve and filter the pages on the current level
            var children = contentLoader.GetChildren<PageData>(levelRootLink);
            children = FilterForVisitor.Filter(children)
                .OfType<PageData>()
                .Where(x => x.VisibleInMenu);

            if (!children.Any())
            {
                return;
            }

            var writer = helper.ViewContext.Writer;

            //Open list element for the current level
            writer.WriteLine("<ul class=\"nav\">");

            //Do some magic
            var indexedChildren = children
                .Select((page, index) => new { index, page })
                .ToList();

            foreach (var levelItem in indexedChildren)
            {
                var page = levelItem.page;
                var partOfCurrentBranch = path.Any(x => x.CompareToIgnoreWorkID(levelItem.page.ContentLink));

                if (partOfCurrentBranch)
                {
                    //Highlight pages in the current branch, including the currently viewed page.
                    writer.WriteLine("<li class=\"active\">");
                }
                else
                {
                    writer.WriteLine("<li>");
                }
                writer.WriteLine(helper.PageLink(page).ToHtmlString());

                if (partOfCurrentBranch)
                {
                    //Recursively render the level below this one
                    RenderSubNavigationLevel(
                        helper,
                        page.ContentLink,
                        path,
                        contentLoader);
                }
                writer.WriteLine("</li>");
            }

            writer.WriteLine("</ul>");
        }
    }
}