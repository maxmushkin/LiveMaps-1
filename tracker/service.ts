import { TrackerItem, TrackerData } from "./item";
import { busRoutes, fieldRoutesLvl1, fieldRoutesLvl2, Route } from "./data";

interface TrackerItemsById {
  [itemId: string]: TrackerItem;
}

const generateTrackerItems = (routes: Route[]): TrackerItemsById => {
  return routes.reduce<TrackerItemsById>((acc, item, idx) => {
    const dataItem = new TrackerItem(`Field worker ${idx}`, item);
    acc[dataItem.id] = dataItem;
    return acc;
  }, {});
};

export class TrackingService {
  updateInterval: NodeJS.Timeout;

  constructor(
    private data: { [locationId: string]: TrackerItemsById | undefined }
  ) {
    this.updateInterval = setInterval(() => {
      Object.values(this.data).forEach((itemsById) => {
        Object.values(itemsById!).forEach((dataItem) =>
          dataItem.updateLocation()
        );
      });
    }, 1000);
  }

  public getDataItems(locationId: string): TrackerData[] | undefined {
    if (this.data[locationId] !== undefined) {
      return Object.values(this.data[locationId]!);
    }
  }
}

const fieldData = {
  "/pugetsound/westcampus/b121/l01": generateTrackerItems(fieldRoutesLvl1),
  "/pugetsound/westcampus/b121/l02": generateTrackerItems(fieldRoutesLvl2),
};

const shuttleData = {
  "/pugetsound/westcampus": generateTrackerItems(busRoutes),
};

export const fieldTrackerService = new TrackingService(fieldData);
export const shuttleTrackerService = new TrackingService(shuttleData);
