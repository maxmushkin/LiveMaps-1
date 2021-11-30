import { array, ArraySchema } from 'yup';

import { strictNumber } from '../utils/validationUtils';

export interface MapPosition {
  longitude: number;
  latitude: number;
}

export type Polygon = [number, number][];

const positionSchema: ArraySchema<number> = array().of(strictNumber).min(2).max(2).defined().nullable(false);
export const polygonSchema: ArraySchema<number[]> = array().of(positionSchema).defined().nullable(false).min(3);

export interface MarkerData {
  title?: string;
  description?: string;
  position?: MapPosition;
  url?: string;
  roomName?: string;
}

export interface TrackerData {
  id: string;
  name: string;
  position: MapPosition;
  color: string;
  icon: string;
  location: string;
}

export interface RouteData {
  summary: Summary;
  legs:  Leg[];
}

export interface Leg {
  summary: Summary;
  points:  Point[];
}

export interface Point {
  latitude:  number;
  longitude: number;
}

export interface Summary {
  travelMode:          string;
  lengthInMeters:      number;
  travelTimeInSeconds: number;
  startLevel:          number;
  endLevel:            number;
}

export interface WebSocketConnectionMessage{
  ConnectionId : string;
}

export interface MapObject {
  polygon?: Polygon;
}