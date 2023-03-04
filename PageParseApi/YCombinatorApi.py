import requests
from BrianTools import ApiError, IsNullOrEmpty
import json

TOP_STORIES_URL = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty"
STORY_URL_FORMAT = "https://hacker-news.firebaseio.com/v0/item/{0}.json?print=pretty"
STORY_LINK_FORMAT = "https://news.ycombinator.com/item?id={0}"

def GetYCombinatorData():
    response = requests.get(TOP_STORIES_URL)

    if response is None or response.status_code != 200:
        return ApiError("Failed to get top stories response from YCombinator API")

    if IsNullOrEmpty(response.text):
        return ApiError("YCombinator top stories API response was null or empty")

    try:
        story_ids = json.loads(response.text)[:5]
    except Exception:
        return ApiError("YCombinator top stories deserialization exception")
    
    if len(story_ids) == 0:
        return ApiError("YCombinator top stories deserialized response was null or empty")

    stories = []

    for story_id in story_ids:
        story_url = STORY_URL_FORMAT.format(story_id)
        story_response = requests.get(story_url)
        if story_response is None or story_response.status_code != 200:
            continue

        if IsNullOrEmpty(story_response.text):
            continue

        try:
            story = json.loads(story_response.text)    
        except Exception:
            continue

        if story is None or "title" not in story:
            continue

        title = story["title"]
        url = STORY_LINK_FORMAT.format(story_id)
        if IsNullOrEmpty(title):
            continue

        result = { "title": title, "url": url }
        stories.append(result)
    
    if len(stories) == 0:
        return ApiError("YCombinator failed to get any stories")

    return { "stories": stories }

if __name__ == "__main__":
    data = GetYCombinatorData()
    print(data)
