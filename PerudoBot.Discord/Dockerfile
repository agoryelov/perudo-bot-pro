FROM python:3

RUN mkdir -p /usr/src/app
COPY . /usr/src/app
WORKDIR /usr/src/app

RUN pip install -r ./requirements.txt

RUN apt-get update && \ 
    apt-get install -y ffmpeg libffi-dev libnacl-dev python3-dev

CMD ["python", "./main.py" ]