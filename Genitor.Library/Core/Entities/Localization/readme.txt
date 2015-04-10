Myšlenka spočívá v tom, že se obecně pracuje s entitama v DefaultLanguage, ty jsou také přímo uložené v odpovídajících tabulkách.
Pokud chci Entitu v jiném jazyku, použiju ILocalizationDataContext.Localize(entity, languageCode), které do všech properties,
které jsou označené atributem [Localizable], načte hodnoty z Localizations tabulky, tím pádem je entita přeložená.
Lokalizace je další dotaz do DB.
