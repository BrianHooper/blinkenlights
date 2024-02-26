// Auto-generated with quicktype.io
//
// To parse this data:
//
//   import { Convert, APIStatus } from "./file";
//
//   const aPIStatus = Convert.toAPIStatus(json);
//
// These functions will throw an error if the JSON doesn't
// match the expected interface, even if the JSON is valid.

export interface APIStatus {
    Items?: APIStatusItem[];
}

export interface APIStatusItem {
    ApiType?: number;
    Name?: string;
    LastUpdate?: string;
    NextValidRequestTime?: null;
    State?: number;
    Source?: number;
    Status?: string;
}

// Converts JSON strings to/from your types
export class Convert {
    public static toAPIStatus(json: string): APIStatus {
        return JSON.parse(json);
    }

    public static APIStatusToJson(value: APIStatus): string {
        return JSON.stringify(value);
    }
}
