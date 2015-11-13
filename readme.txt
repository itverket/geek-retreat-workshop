Komme igang hurtig med front end:

Dersom du/teamet �nsker, kan dere lime inn f�lgende filer fra Documents-mappen inn i Web-API prosjektet deres.

1.index.html -> wwwroot

	Enkelt start-view hvor man kan s�ke p� tweets og vise tweets. Bruker knockout og pagerjs

2. viewmodels.js -> wwwroot

	Inneholder 4 Knockout viewmodels: 

	TweetrSearchViewModel
	SearchCriteriaViewModel
	SearchResultViewModel
	TweetViewModel

	Disse kan splittes opp i separate filer om det �nskes. Pass p� referanser i index.html

3. knockout.viewmodel.min.js -> wwwroot/externalScripts 
    (kan selvf�lgelig navngi mappene som dere vil. 
	Pass p� refernaser i index.html)

4. AzureSearchResult.cs -> ViewModels

	Det opprinnelige s�keresultatet ekstender IEnumerable, i tillegg til � ha nye properties.
	Vi klarer ikke � opprette tilsvarende objekter i js, derfor mapper vi om resultater til AzureSearchResult.

5. gulpfile.js

	Kopier innhold eller kopier fil. 
	Knockout, pagerjs og jquery-navn m� inn i bower-variabelen i 'copy'-tasken.

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






		








