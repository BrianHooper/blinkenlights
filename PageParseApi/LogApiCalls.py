import uvicorn
from fastapi import FastAPI, Request, Body
import os

def WriteLog(*args):
    items = "\t".join([str(a) for a in args])
    output = f"{items}\r\n"
    with open(filename, "a", encoding="utf-8") as infile:
        infile.write(output)

filename = "logfile.tsv"
app = FastAPI()

@app.post("/v4/trackings")
async def trackings(request: Request, data = Body(...)):
    WriteLog(request.headers, data)

if __name__ == "__main__":
    uvicorn.run("LogApiCalls:app", host="localhost", port=5123, log_level="info")