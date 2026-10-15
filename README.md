# IoT Sensor Data Management System

An IoT project integrating an ESP32-based device, a database, and an ASP.NET Core MVC web application for sensor data collection, storage, visualization, filtering, and export.

The system demonstrates the integration of embedded devices, database management, web development, and Linux-based deployment in a single end-to-end project.

## Overview

The project consists of three main components:

- **ESP32:** Collects sensor data and transmits records for storage.
- **Database (SENSOR_DB):** Stores sensor records for subsequent retrieval and analysis.
- **ASP.NET Core MVC Web Application:** Provides a web interface for browsing, filtering, visualizing, and exporting the collected data.

The application was deployed on a Linux virtual machine and configured for remote access using Cloudflare Tunnel.

## Key Features

### IoT Data Acquisition
- Integrated an ESP32-based device into the data collection workflow.
- Supported the addition of sensor records to the database.
- Enabled access to collected measurements through the web application.

### Database Management
- Implemented database-backed retrieval and presentation of sensor records.
- Supported viewing complete datasets and filtered subsets.
- Organized measurement data for further analysis and visualization.

### Web Application
- Developed an ASP.NET Core MVC web application.
- Implemented tabular data presentation and navigation between application views.
- Added user-selectable filters for retrieving relevant records.
- Included a student information view.

### Data Visualization
- Implemented time-series graphs for exploring sensor measurements.
- Enabled users to select measurement types, positions, and time intervals.
- Provided a visual representation of sensor data over time.

### Data Export
- Implemented data export to Excel-compatible formats and XML.
- Supported exporting complete or filtered datasets for further processing.

### Deployment and Remote Access
- Deployed the web application on a Linux virtual machine.
- Configured a web server and verified access over the local network.
- Set up Cloudflare Tunnel to provide remote access to the application.

## System Architecture

```text
ESP32
  |
  | Sensor data
  v
SENSOR_DB Database
  ^
  | Data retrieval
  |
ASP.NET Core MVC
  |
  +-- Tabular Data & Filtering
  +-- Time-Series Visualization
  +-- Excel-Compatible / XML Export
  |
  v
Web Server on Linux VM
  |
  v
Cloudflare Tunnel
  |
  v
Remote Browser Access
```

## Technologies Used

- **Embedded / IoT:** ESP32
- **Backend:** C#, ASP.NET Core MVC
- **Database:** SENSOR_DB
- **Data Visualization:** Time-series charts
- **Data Formats:** Excel-compatible files, XML
- **Operating System:** Linux
- **Networking & Deployment:** Virtual machine, web server, Cloudflare Tunnel

## Skills Demonstrated

- Embedded systems and IoT integration
- Database-backed application development
- C# and ASP.NET Core MVC
- Data filtering, visualization, and export
- Linux deployment and server configuration
- Network access and remote application deployment
- Integration of hardware, software, and data storage

## Project Type

Academic project.

The project demonstrates an end-to-end workflow connecting an embedded IoT device with a database-driven web application, from sensor data acquisition to remote visualization and export.
