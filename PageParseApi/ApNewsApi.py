import requests
from bs4 import BeautifulSoup
from BrianTools.Tools import Engine, IsNullOrEmpty, ApiError
import lxml

def GetAPNewsStories():
    APITYPE = "APNEWS"
    url = "https://news.google.com/rss/search?q=when:24h+allinurl:apnews.com&hl=en-US&gl=US&ceid=US:en"

    payload = ""
    headers = {"User-Agent": "insomnia/2023.5.8"}

    try:
        response = requests.request("GET", url, data=payload, headers=headers)
    except Exception as e:
        return ApiError(f"{APITYPE} request exception: {e}")
    
    if response is None:
        return ApiError(f"{APITYPE} request failed with null response")
    if response.status_code != 200:
        return ApiError(f"{APITYPE} request failed with status {response.status_code}")
    if IsNullOrEmpty(response.text):
        return ApiError(f"{APITYPE} request failed with empty text response")
    
    soup = BeautifulSoup(response.text, features="xml")
    items = soup.find_all("item")
    stories = []
    for item in items:
        title = item.find("title")
        if title is None or title.text is None or len(title.text) == 0:
            continue
        link = item.find("link")
        if link is None or link.text is None or len(link.text) == 0:
            continue
        result = { "title": title.text, "url": link.text }
        stories.append(result)
        if len(stories) >= 3:
            break

    if len(stories) == 0:
        return ApiError(f"{APITYPE} failed to process any stories")
    
    return {
        "stories": stories
    }

if __name__ == "__main__":
    data = GetAPNewsStories()
    pass