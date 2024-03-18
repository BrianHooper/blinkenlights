import requests
from bs4 import BeautifulSoup
from BrianTools.Tools import Engine, IsNullOrEmpty, ApiError
import lxml

def GetNprStories():
    url = "https://text.npr.org/"

    payload = ""
    headers = {"User-Agent": "insomnia/2023.5.8"}

    try:
        response = requests.request("GET", url, data=payload, headers=headers)
    except Exception as e:
        return ApiError(f"NPR request exception: {e}")
    
    if response is None:
        return ApiError(f"NPR request failed with null response")
    if response.status_code != 200:
        return ApiError(f"NPR request failed with status {response.status_code}")
    if IsNullOrEmpty(response.text):
        return ApiError(f"NPR request failed with empty text response")
    
    soup = BeautifulSoup(response.text, "html.parser")
    container = soup.find("div", {"class": "topic-container"})
    items = container.find_all("li")
    stories = []
    for item in items:
        title = item.string
        if title is None or len(title) == 0:
            continue

        link = item.find("a")
        if link is None or link.text is None:
            continue
        href = link.attrs["href"]
        if href is None or len(href) == 0:
            continue

        result = { "title": title, "url": f"https://www.npr.org{href}" }
        stories.append(result)
        if len(stories) >= 3:
            break

    if len(stories) == 0:
        return ApiError(f"NPR failed to process any stories")
    
    return {
        "stories": stories
    }

if __name__ == "__main__":
    data = GetNprStories()
    pass