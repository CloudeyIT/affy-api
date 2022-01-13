global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Threading.Tasks;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.EntityFrameworkCore;
global using Mapster;
global using MoreLinq.Extensions;
using System.Reflection;

[assembly: AssemblyVersion(
    ThisAssembly.Git.BaseVersion.Major +
    "." +
    ThisAssembly.Git.BaseVersion.Minor +
    "." +
    ThisAssembly.Git.BaseVersion.Patch
)]
[assembly: AssemblyFileVersion(
    ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.SemVer.Patch
)]
[assembly: AssemblyInformationalVersion(
    ThisAssembly.Git.Commits +
    "-" +
    ThisAssembly.Git.Branch +
    "+" +
    ThisAssembly.Git.Commit
)]
[assembly: AssemblyCompanyAttribute("El Nino")]
[assembly: AssemblyProductAttribute("Affy")]
[assembly: AssemblyTitleAttribute("Affy.Api")]