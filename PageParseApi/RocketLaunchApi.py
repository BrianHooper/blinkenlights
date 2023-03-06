import os
from pathlib import Path
from datetime import date
from BrianTools import Engine, ApiError
import json

SOURCE_URL = "https://www.spacelaunchschedule.com"

def GetRocketData():
    today = str(date.today())

    engine = Engine()
    soup, html = engine.GetSoup(SOURCE_URL)
    if soup is None:
        return ApiError("Failed to get rocket page")
    
    articles = soup.find_all("article")
    if articles is None or len(articles) == 0:
        return ApiError("Failed to get any articles")

    stories = []
    for article in articles[:5]:
        if article is None:
            continue
        title_strs = list(article.strings)
        if len(title_strs) == 0:
            continue

        title = " - ".join(title_strs)

        story = { "title": title, "url": "https://www.spacelaunchschedule.com/" }
        stories.append(story)

    if len(stories) == 0:
        return ApiError("Failed to find any upcoming launches")

    data = {
        "date": today,
        "stories": stories,
    }

    return data

if __name__ == "__main__":
    GetRocketData()