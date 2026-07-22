from fastapi import FastAPI

app = FastAPI(title="Sports Gurukul AI Service")

@app.get("/health")
def health():
    return {"status":"ok"}

@app.get("/ai/coach")
def coach():
    return {"message":"AI Coach endpoint placeholder"}
