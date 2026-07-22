from fastapi import FastAPI

app = FastAPI(title="Sports Gurukul AI Service", version="0.1.0")


@app.get("/health")
def health_check():
    return {"status": "ok"}


@app.get("/ai/coach")
def coach_placeholder():
    return {"message": "AI Coach endpoint placeholder"}
