from datetime import date
from BrianTools.Tools import Engine, ApiError, IsNullOrEmpty

def GetWikiPicOfTheDay():
    engine = Engine()
    soup, html = engine.GetSoup("https://en.wikipedia.org/wiki/Wikipedia:Picture_of_the_day")    
    if soup is None:
        return ApiError("Failed to get wikipedia potd page")
    
    imageSoup = soup.find("img", {"class": "mw-file-element"})
    if imageSoup is None or imageSoup.attrs is None or "src" not in imageSoup.attrs:
        return ApiError("Failed to find image on page")
    
    source = imageSoup.attrs["src"]
    if IsNullOrEmpty(source):
        return ApiError("Failed to extract image source")
    source = f"https:{source}"

    title = imageSoup.attrs["alt"]
    if title is None or len(title) == 0:
        return ApiError("Failed to extract image title")

    data = {
        "Title": title,
        "Source": source,
        "Url": "https://en.wikipedia.org/wiki/Wikipedia:Picture_of_the_day"
    }
    return data

if __name__ == "__main__":
    data = GetWikiPicOfTheDay()
    print(data)
