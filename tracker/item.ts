import { v4 as UUID } from "uuid";

let counter = 1;

export interface MapPosition {
  longitude: number;
  latitude: number;
}

export interface TrackerData {
  id: string;
  name: string;
  position: MapPosition;
}

export class TrackerItem implements TrackerData {
  private idx?: number;
  private reverse: boolean = false;
  public readonly id = UUID();

  public position: MapPosition;

  constructor(
    public readonly name = `Field worker ${counter++}`,
    private locations: [number, number][]
  ) {
    this.position = this.updateLocation();
  }

  private getNextIdx(): number {
    if (this.locations.length < 2) {
      return 0;
    }

    if (this.idx === undefined) {
      this.idx = 0;
      return this.idx;
    }

    const lastIdx = this.locations.length - 1;
    if (this.reverse) {
      if (this.idx > 0) {
        return --this.idx;
      }

      this.reverse = false;
      return ++this.idx;
    }

    // Direct
    if (this.idx < lastIdx) {
      return ++this.idx;
    }

    this.reverse = true;
    return --this.idx;
  }

  public updateLocation() {
    const nextIdx = this.locations.length === 1 ? 0 : this.getNextIdx();
    const [longitude, latitude] = this.locations[nextIdx];
    this.position = { latitude, longitude };
    return this.position;
  }
}
