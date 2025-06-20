// k6 Load Test Script for ASP.NET Core API
import http from 'k6/http';
import { check, sleep } from 'k6';

// Define test options: ramp-up, steady state, ramp-down
export let options = {
    stages: [
        { duration: '30s', target: 50 },  // ramp-up to 50 users
        { duration: '1m', target: 50 },   // stay at 50 users
        { duration: '30s', target: 0 },   // ramp-down to 0 users
    ],
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
        http_req_failed: ['rate<0.01'],   // error rate should be less than 1%
    },
};

const BASE_URL = 'https://api.example.com'; // Replace with your API URL

export default function () {
    // Example GET request to fetch a resource list
    let res = http.get(
