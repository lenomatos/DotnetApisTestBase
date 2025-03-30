import http from "k6/http";
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { check, sleep } from "k6";

// Configuration
export const options = {
  stages: [
    { duration: "30s", target: 25 }, // Ramp-up to 25 users
    { duration: "1m", target: 50 }, // Hold 25 users
    { duration: "1m", target: 100 }, // Spike to 50 users
    { duration: "30s", target: 50 }, // Ramp-down
    { duration: "30s", target: 0 }, // Ramp-down
  ],
  thresholds: {
    http_req_failed: ["rate<0.01"], // Fail if >1% requests fail
    http_req_duration: ["p(95)<500"], // 95% of requests <500ms
  },
};

// Test execution
export default function () {
  const baseUrl = "https://localhost:44312"; // Update if hosted elsewhere

  // Test 1: CheckNumberType endpoint
  const numRes = http.get(`${baseUrl}/Prime/checknumber/42`);
  check(numRes, {
    "CheckNumberType status is 200": (r) => r.status === 200,
    "CheckNumberType returns Even": (r) => r.body.includes("Even"),
  });

  // Test 2: GenerateRandomNumberAsync endpoint (if applicable)
  const randomRes = http.get(`${baseUrl}/Prime/random/1/99`);
  check(randomRes, {
    "GenerateRandomNumberAsync status is 200": (r) => r.status === 200,
  });

  // Test 3: GetProduct endpoint (if applicable)
  const productRes = http.get(`${baseUrl}/Product/3`);
  check(productRes, {
    "GetProduct status is 200": (r) => r.status === 200,
  });

  sleep(1); // Simulate user think time
}

export function handleSummary(data) {
  return {
    "PrimeWebApi.Tests.K6/summary.html": htmlReport(data),
  };
}
