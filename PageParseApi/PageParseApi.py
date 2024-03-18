import uvicorn
from fastapi import FastAPI, Request, Body
import BrianTools
from BrianTools.Tools import Engine, ApiError
import requests
from bs4 import BeautifulSoup
from BrianTools.Tools import Engine, IsNullOrEmpty, ApiError
import lxml
from GoogleCalendarApi import GetCalendar
from WikipediaApi import GetWikipedia
from YCombinatorApi import GetYCombinatorData
from RocketLaunchApi import GetRocketData
from AstronomyApi import GetAstroPicOfTheDay
from PackageTrackingApi import GetTrackingInfo
from IssTrackerApi import GetIssLocationImage
from WikipediaPotd import GetWikiPicOfTheDay
from FlightRadarApi import GetFlightRadarData, GetSingleFlightData
from NprApi import GetNprStories

app = FastAPI()

@app.get("/wikipedia")
def wikipedia():
    try:
        wikiData = GetWikipedia()
        return wikiData
    except Exception as e:
        return ApiError(f"Exception: {e}")
    
@app.get("/rockets")
def rockets():
    try:
        rocketData = GetRocketData()
        return rocketData
    except Exception as e:
        return ApiError(f"Exception: {e}")
    
@app.get("/ycombinator")
def ycombinator():
    try:
        ycombinatorData = GetYCombinatorData()
        return ycombinatorData
    except Exception as e:
        return ApiError(f"Exception: {e}")

@app.get("/googlecalendar")
async def GoogleCalendar(request: Request):
    userAccount = request.headers.get("X-user-account")
    if userAccount is None or len(userAccount) == 0:
        return ApiError("Missing header: X-user-account")
    secret = request.headers.get("X-user-secret")
    if secret is None or len(secret) == 0:
        return ApiError("Missing header: X-user-secret")
    return GetCalendar(userAccount, secret)

@app.get("/astronomypotd")
def astronomy():
    return GetAstroPicOfTheDay()

@app.post("/packagetracking")
async def PackageTracking(requests: Request, Items = Body(...)):
    return GetTrackingInfo(requests.headers, Items)

@app.get("/isstracker")
def isstracker():
    return GetIssLocationImage()

@app.get("/wikipotd")
def wikipotd():
    return GetWikiPicOfTheDay()

@app.get("/flightradar")
def flightradar():
    return GetFlightRadarData()

@app.get("/flightradar/{fid}")
def flightradarsingle(fid: str):
    return GetSingleFlightData(fid)

@app.get("/npr")
def npr():
    try:
        nprData = GetNprStories()
        return nprData
    except Exception as e:
        return ApiError(f"Exception: {e}")

@app.get("/btown")
def btown():
    return GetGenericRssFeedData("https://b-townblog.com/feed/", "btown")

@app.get("/apnews")
def apnews():
    return GetGenericRssFeedData("https://news.google.com/rss/search?q=when:24h+allinurl:apnews.com&hl=en-US&gl=US&ceid=US:en", "apnews")

@app.get("/seattletimes")
def seattletimes():
    return GetGenericRssFeedData("https://www.seattletimes.com/feed/", "seattletimes")

def GetGenericRssFeedData(url, name):
    try:
        data = GetGenericRssfeedStories(url, name)
        return data
    except Exception as e:
        return ApiError(f"Exception: {e}")

def GetGenericRssfeedStories(url, name):
    payload = ""
    headers = {"User-Agent": "insomnia/2023.5.8"}

    try:
        response = requests.request("GET", url, data=payload, headers=headers)
    except Exception as e:
        return ApiError(f"{name} request exception: {e}")
    
    if response is None:
        return ApiError(f"{name} request failed with null response")
    if response.status_code != 200:
        return ApiError(f"{name} request failed with status {response.status_code}")
    if IsNullOrEmpty(response.text):
        return ApiError(f"{name} request failed with empty text response")
    
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
        return ApiError(f"{name} failed to process any stories")
    
    return {
        "stories": stories
    }

if __name__ == "__main__":
    print(f"BrianTools version: {BrianTools.__version__}")
    uvicorn.run("PageParseApi:app", host="127.0.0.1", port=5001, log_level="info")