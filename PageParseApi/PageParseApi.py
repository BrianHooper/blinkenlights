
import uvicorn
from fastapi import FastAPI
from Engine import Engine
import json
from datetime import date

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

@app.get("/other")
def other():
  return {"other!"}

if __name__ == "__main__":
    uvicorn.run("PageParseApi:app", host="127.0.0.1", port=5000, log_level="info")