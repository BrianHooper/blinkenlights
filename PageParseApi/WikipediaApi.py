import os
from pathlib import Path
from datetime import date
from BrianTools.Tools import Engine, ApiError
import json

filename = "WikipediaCache.json"

def GetCachedData(today):
    if not os.path.exists(filename):
        return None
    
    with open(filename, "r", encoding="utf-8") as infile:
        data = json.load(infile)
    if data is None or today not in data:
        return None
    return data[today]

def WriteCachedData(today, data):
    output = {today: data}

    with open(filename, "w", encoding="utf-8") as outfile:
        json.dump(output, outfile)

def GetWikipedia():
    today = str(date.today())

    cache = GetCachedData(today)
    if cache is not None:
        return cache

    engine = Engine()
    soup, html = engine.GetSoup("https://en.wikipedia.org/wiki/Main_Page")
    if soup is None:
        return ApiError("Failed to get wikipedia page")
    In_the_news = soup.find("div", {"id": "mp-itn"})
    if In_the_news is None or In_the_news.children is None:
        return ApiError("Failed to get ITN from wikipedia page")

    itn = []

    for itnchild in In_the_news.children:
        if itnchild.name == "ul":
            itn += [x.text for x in itnchild.find_all("li")]
    # On_This_day = soup.find("div", {"id": "mp-otd"})
    print()

    if len(itn) == 0:
        return ApiError("Failed to get ITN from wikipedia page")

    stories = [{"title": i} for i in itn]

    data = {
        "Date": today,
        "stories": stories,
    }
    
    if len(itn) == 0:
        return ApiError("No ITN items found")

    WriteCachedData(today, data)
    return data

if __name__ == "__main__":
    GetWikipedia()