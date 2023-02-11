
import uvicorn
from fastapi import FastAPI, Request
from BrianTools import Engine, ApiError
from datetime import date
from GoogleCalendarApi import GetCalendar

app = FastAPI()

@app.get("/wikipedia")
def wikipedia():
    engine = Engine()
    soup = engine.GetSoup("https://en.wikipedia.org/wiki/Main_Page")
    In_the_news = soup.find("div", {"id": "mp-itn"})
    itn = []

    for itnchild in In_the_news.children:
        if itnchild.name == "ul":
            itn += [x.text for x in itnchild.find_all("li")]
    # On_This_day = soup.find("div", {"id": "mp-otd"})
    print()

    today = str(date.today())
    data = {
        "Date": today,
        "InTheNews": itn,
    }
    return data

@app.get("/googlecalendar")
async def GoogleCalendar(request: Request):
    userAccount = request.headers.get("X-user-account")
    if userAccount is None or len(userAccount) == 0:
        return ApiError("Missing header: X-user-account")
    secret = request.headers.get("X-user-secret")
    if secret is None or len(secret) == 0:
        return ApiError("Missing header: X-user-secret")
    return GetCalendar(userAccount, secret)
    
if __name__ == "__main__":
    uvicorn.run("PageParseApi:app", host="127.0.0.1", port=5001, log_level="info")