import process from "process";

import cors from "cors";
import express, { Router, RequestHandler } from "express";
import expressWs, { WebsocketRequestHandler } from "express-ws";
import WebSocket, { OPEN } from "ws";

import {
  fieldTrackerService,
  shuttleTrackerService,
  TrackingService,
} from "./tracker/service";

let listenPort: number = 3001;
const envPort = process.env["HTTP_PORT"];
if (envPort) {
  try {
    const parsed = parseInt(envPort, 10);
    if (!isNaN(parsed)) {
      listenPort = parsed;
    }
  } catch {
    // Couldn't parse port - just ignore
  }
}

const makeDataHandler = (tracker: TrackingService): RequestHandler => {
  return (req, res) => {
    const data = tracker.getDataItems(req.path);
    return data ? res.json(data) : res.status(400);
  };
};

const makeDataTracker = (tracker: TrackingService): WebsocketRequestHandler => {
  return (socket, req, next) => {
    const locId = req.path.split("/").slice(0, -1).join("/");

    let updateInterval: NodeJS.Timeout | undefined;

    socket.on("close", (code, reason) => {
      console.log(`Connection closed by client: ${code}: ${reason}`);
      if (updateInterval) {
        clearInterval(updateInterval);
      }
    });

    socket.on("error", (err) => {
      console.error(`Connection error: ${err}`);
      if (updateInterval) {
        clearInterval(updateInterval);
      }
    });

    updateInterval = setInterval(
      (socket: WebSocket) => {
        if (socket.readyState === OPEN) {
          const items = tracker.getDataItems(locId);
          if (items) {
            const update = items.map(({ id, name, position }) => ({
              id,
              name,
              position,
            }));
            socket.send(JSON.stringify(update));
          }
        }
      },
      1000,
      socket
    );
  };
};

const requestLogger: RequestHandler = (req, res, next) => {
  next();
  console.log(`${req.method} ${req.path} ${res.statusCode}`);
};

const app = express();

expressWs(app)
  .app.use(cors())
  .use(requestLogger)
  .use("/field", makeDataHandler(fieldTrackerService))
  .use("/shuttle", makeDataHandler(shuttleTrackerService))
  .use(
    "/field_tracker",
    Router().ws("/*", makeDataTracker(fieldTrackerService))
  )
  .use(
    "/shuttle_tracker",
    Router().ws("/*", makeDataTracker(shuttleTrackerService))
  )
  .listen(listenPort);

console.log(`Listening on http://localhost:${listenPort}`);
