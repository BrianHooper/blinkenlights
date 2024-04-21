from datetime import date
from BrianTools.Tools import Engine, ApiError, IsNullOrEmpty

def GetPeopleInSpace():
    engine = Engine()
    soup, html = engine.GetSoup("https://en.wikipedia.org/wiki/Low_Earth_orbit")    
    if soup is None:
        return ApiError("Failed to get wikipedia potd page")
    
    subSoup = soup.find("div", {"id": "People_currently_in_low_Earth_orbit"})
    if subSoup is None or subSoup.attrs is None or "src" not in subSoup.attrs:
        return ApiError("Failed to find image on page")
    
    subsubsoup = subSoup.find_all("table")

    source = subSoup.attrs["src"]
    if IsNullOrEmpty(source):
        return ApiError("Failed to extract image source")
    source = f"https:{source}"

    title = subSoup.attrs["alt"]
    if title is None or len(title) == 0:
        return ApiError("Failed to extract image title")

    data = {
        "Title": title,
        "Source": source,
        "Url": "https://en.wikipedia.org/wiki/Wikipedia:Picture_of_the_day"
    }
    return data

if __name__ == "__main__":
    data = GetPeopleInSpace()
    print(data)
