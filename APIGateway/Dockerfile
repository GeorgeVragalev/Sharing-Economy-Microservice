# Use an official Python runtime as a base image
FROM python:3.9-slim-buster

# Set environment variables
ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

# Set the working directory inside the container
WORKDIR /APIGateway

# Install system dependencies
RUN apt-get update && \
    apt-get install -y --no-install-recommends gcc apt-utils && \
    rm -rf /var/lib/apt/lists/*

# Install any needed packages specified in requirements.txt
COPY requirements.txt /APIGateway/

RUN pip install --upgrade pip
RUN pip install --no-cache-dir -r requirements.txt

# Copy the current directory contents into the container at /APIGateway
COPY . /APIGateway/

# Make port 5000 available to the world outside this container
EXPOSE 5000

# Run app.py when the container launches
CMD ["python", "-m", "flask", "run", "--host=0.0.0.0"]
