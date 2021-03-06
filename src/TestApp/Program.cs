﻿using ClickTwice.Publisher.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ClickTwice.Handlers.AppDetailsPage;
using ClickTwice.Publisher.Core.Handlers;
using ClickTwice.Publisher.Core.Loggers;
using ClickTwice.Publisher.MSBuild;
using ClickTwice.Templating;

namespace TestApp
{
    internal class Program
    {
        private static string DefaultProjectPath { get; set; } =
        //@"C:\Users\UCRM4\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; // ATO
        //@"C:\Users\alist_000\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; //others
        //@"C:\Users\alist\Source\ACN\TFSShowcase\src\ScreenshotReportCreator\ScreenshotReportCreator.csproj"; // Zenbook
        //@"C:\Users\alist_000\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";
        @"C:\Users\alist\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";
        //@"C:\Users\UCRM4\Source\ACN\myTaxFramework\FormDocuments\DocumentConversion\DocumentConversion.csproj";

        private static void Main(string[] args)
        {
            var packager = new TemplatePackager("ClickTwice.Templates.SolidState", "0.0.1", "Alistair Chapman",
                "ClickTwice Template using the HTML5UP Solid State design");
            //var package = packager.Package(@"C:\Users\alist\Source\ClickTwice\src\ClickTwice.Templates.SolidState", PackagingMode.VisualStudio);
            var package = packager.Package(@"C:\Users\alist\Source\TEMP\solid-state", PackagingMode.Minimal);
            var handler = new AppDetailsPageHandler(package);
            if (args.Any())
            {
                DefaultProjectPath = args.First();
            }
            var log = new ConsoleLogger();
            var file = new FileLogger();
            var info = new AppInfoManager();
            var infoHandler = new AppInfoHandler();
            BuildInfo(info);
            var mgr = new PublishManager(DefaultProjectPath, InformationSource.Both)
            {
                Platform = "AnyCPU",
                Configuration = "Debug",
                InputHandlers = new List<IInputHandler> {infoHandler},
                OutputHandlers = new List<IOutputHandler> {infoHandler, new PublishPageHandler(), new InstallPageHandler("install.htm"), handler },
                Loggers = new List<IPublishLogger> { log, file }
            };
            // ReSharper disable once RedundantArgumentDefaultValue
            var path = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")));
            var result = mgr.PublishApp(path.FullName, behaviour: PublishBehaviour.CleanFirst);
            //var manager = new ManifestManager(DefaultProjectPath, path.FullName, InformationSource.Both);
            //var manifest = manager.CreateAppManifest();
            //var cltw = manager.DeployManifest(manifest);
            Process.Start(path.FullName);
            Console.WriteLine(result.Select(r => $"{r.Handler.Name} - {r.Result} - {r.ResultMessage}" + Environment.NewLine));
        }

        private static void BuildInfo(AppInfoManager info)
        {
            info.AddAuthor("Alistair Chapman");
        }
    }
}