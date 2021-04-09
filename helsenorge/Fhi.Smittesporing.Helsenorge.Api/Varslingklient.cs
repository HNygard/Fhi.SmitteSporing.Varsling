using Fhi.Smittesporing.Varsling.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell;
using Fhi.Smittesporing.Varsling.Felles.Applikasjonsmodell.Innsyn;
using Fhi.Smittesporing.Varsling.Intern.Applikasjonsmodell;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Fhi.Smittesporing.Helsenorge.Api
{
    public class Varslingklient
    {
        private readonly HttpClient _httpClient;
        public Varslingklient(HttpClient httpclient)
        {
            _httpClient = httpclient;
        }

        public async Task<IEnumerable<InnsynLoggHelsenorgeAm>> HentInnsynlogg(HelsenorgeToken token)
        {
            var relativeurl = "logg/"
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.AktorFodselsnummer), token.AktorSubjektPersonIdent ?? string.Empty)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.AktorName), token.AktorSubjectName ?? string.Empty)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.Fodselsnummer), token.SubjektPersonIdent)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.Telefonnummer), token.Telefonnummer);

            var response = await _httpClient.GetAsync(relativeurl);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<IEnumerable<InnsynLoggHelsenorgeAm>>();

            return result;
        }

        public async Task<InnsynHendelserHelsenorgeAm> HentHelsenorgeHendelser(HelsenorgeToken token)
        {
            var relativeurl = "hendelser/"
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.AktorFodselsnummer), token.AktorSubjektPersonIdent ?? string.Empty)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.AktorName), token.AktorSubjectName ?? string.Empty)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.Fodselsnummer), token.SubjektPersonIdent)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.Telefonnummer), token.Telefonnummer);

            var response = await _httpClient.GetAsync(relativeurl);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<InnsynHendelserHelsenorgeAm>();

            return result;
        }

        public async Task<InnsynHelsenorgeAm> HentInnsynHelsenorge(HelsenorgeToken token)
        {
            var relativeurl = ""
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.AktorFodselsnummer), token.AktorSubjektPersonIdent ?? string.Empty)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.AktorName), token.AktorSubjectName ?? string.Empty)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.Fodselsnummer), token.SubjektPersonIdent)
                .AddQueryString(nameof(InnsynHelsenorgeRequestAm.Telefonnummer), token.Telefonnummer);

            var response = await _httpClient.GetAsync(relativeurl);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsAsync<InnsynHelsenorgeAm>();

            return result;
        }
    }
}
