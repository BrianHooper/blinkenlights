from datetime import date
from BrianTools.Tools import Engine, ApiError, IsNullOrEmpty

def GetPicOfTheDay():
    engine = Engine()
    soup, html = engine.GetSoup("https://apod.nasa.gov/apod/astropix.html")    
    if soup is None:
        return ApiError("Failed to get astropix page")
    
    centersSoupLst = soup.find_all("center")
    if centersSoupLst is None or len(centersSoupLst) < 2:
        return ApiError("Failed to find required content on page")
    
    imageSoup = centersSoupLst[0].find("img")
    if imageSoup is None or imageSoup.attrs is None or "src" not in imageSoup.attrs:
        return ApiError("Failed to find image on page")
    
    source = imageSoup.attrs["src"]
    if IsNullOrEmpty(source):
        return ApiError("Failed to extract image source")
    source = f"https://apod.nasa.gov/apod/{source}"

    titleSoup = centersSoupLst[1].find("b")
    if titleSoup is None:
        return ApiError("Failed to extract image title")
    title = titleSoup.text
    if IsNullOrEmpty(title):
        return ApiError("Title is null or empty")

    data = {
        "Title": title,
        "Source": source,
        "Url": "https://apod.nasa.gov/apod/astropix.html"
    }
    return data

if __name__ == "__main__":
    data = GetPicOfTheDay()
    print(data)
