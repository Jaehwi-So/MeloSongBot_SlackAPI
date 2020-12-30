const request = require('request');

const api_request = async (host, path, query, method) => {
  const url = "" + host + "/" + path + query;
  return new Promise((resolve, reject) => {
    if(method == 'GET'){
      request.get(url, (error, response, body) => {
        if (!error && response.statusCode == 200) {
          resolve(body);
        }
        else{
          reject(new Error(error));
        }
      });
    }
    else if(method == 'POST'){
      request.post(url, (error, response, body) => {
        if (!error && response.statusCode == 200) {
          resolve(body);
        }
        else{
          reject(new Error(error));
        }
      });
    }
  });
}
module.exports = api_request;