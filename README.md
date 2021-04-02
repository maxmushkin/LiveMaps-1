# Tracking layer mock server

This is a server to provide sample data for tracking layers for https://github.com/Azure-Samples/LiveMaps.

The data that is served to the app is placed in `tracker/data.ts` and contains
2 datasets for `/pugetsound/westcampus/b121/l01` location - campus shuttles and
field workers. One element of each dataset contains a route that is streamed to
the app through websocket while other elements are static.

To emulate points moving along the routes the app creates a class instance per
each route that "knows" how to provide the next point of the route and then
polls these instances for the new points every `n` (`n=1` by default) seconds.

This allows serer to provide syncronized updates to all connected clients.

## Running the server

To run the server, first install dependencies

    npm install

and then run

    npm run dev

## Deployment

If you want to deploy this app somewhere else you may want to build it first
with `npm run build` and use build output from `./build` folder for deployment
(e.g. by running `npm start` command which is configured to do so).

The repository is already configured to deploy to Azure App Service (see
provided `web.config`, `.deployment` and `deploy.sh` files) so you can use Azure
CLI to deploy the app to your subscription.
