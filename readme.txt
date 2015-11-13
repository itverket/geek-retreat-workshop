Komme igang hurtig med front end:

Dersom du/teamet ønsker, kan dere lime inn følgende filer fra Documents-mappen inn i Web-API prosjektet deres.

1.index.html -> wwwroot

	Enkelt start-view hvor man kan søke på tweets og vise tweets. Bruker knockout og pagerjs

2. viewmodels.js -> wwwroot

	Inneholder 4 Knockout viewmodels: 

	TweetrSearchViewModel
	SearchCriteriaViewModel
	SearchResultViewModel
	TweetViewModel

	Disse kan splittes opp i separate filer om det ønskes. Pass på referanser i index.html

3. knockout.viewmodel.min.js -> wwwroot/externalScripts 
    (kan selvfølgelig navngi mappene som dere vil. 
	Pass på refernaser i index.html)

4. AzureSearchResult.cs -> ViewModels

	Det opprinnelige søkeresultatet ekstender IEnumerable, i tillegg til å ha nye properties.
	Vi klarer ikke å opprette tilsvarende objekter i js, derfor mapper vi om resultater til AzureSearchResult.

5. gulpfile.js

	Kopier innhold eller kopier fil. 
	Knockout, pagerjs og jquery-navn må inn i bower-variabelen i 'copy'-tasken.

Andre fil-endringer:

	a. Endre AzureSearchService sin searchmetode:

	public virtual AzureSearchResult Search(
            string query, string username = null)
        {
            var result = _indexClient.Documents.Search<FlattendTweet>(
                query,
                new SearchParameters
                {
                    OrderBy = new List<string> { "CreatedAt" },
                    Filter = string.IsNullOrEmpty(username)
                        ? null
                        : $"Username eq '{username}'",
                    Facets = new List<string> { "HashTags"},
                    IncludeTotalResultCount = true
                });

            return new AzureSearchResult
            {
                Count = result.Count,
                Facets = result.Facets,
                Tweets = result.Results
            };
        }


	b. Legg til Knockout, Pagerjs og jquery i bower.json:

				"pagerjs": "*",
				"knockout": "*",
				"jquery": "*"

	c. Kopier filene fra bower_components til wwwroot/lib:






		








