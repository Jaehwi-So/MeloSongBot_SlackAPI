const Slack = require('slack-node'); 
const schedule = require('node-schedule');
const requestAPI = require('./src/apiManager.js');
const dotenv = require('dotenv');
const stringSimilarity = require('string-similarity');
var { RTMClient } = require('@slack/rtm-api');
const { jsonToString } = require('./src/jsonParser.js');

dotenv.config();
const slackToken = process.env.SLACK_TOKEN || 'SLACK_TOKEN';
const apiHost = process.env.API_HOST || 'SLACK_TOKEN';
const slack = new Slack(slackToken);

const sendMessage = async (message, channel) => {
  slack.api('chat.postMessage', {
    username: 'MeloBot',  
    text: message,
    channel: channel,  
    icon_emoji: 'slack'   
  }, function(err, response){
    console.log(response);
  });
}

const rtm = new RTMClient(slackToken);

rtm.on('message', async (event) => {
  const text = event.text;
  const channel = event.channel;
  const new_detect_query_rate50 = ['최신', '최신곡', '최근', '신곡'];
  const new_detect_query_rate70 = ['차트인', '차트 인'];
  let matches_50 = stringSimilarity.findBestMatch(text, new_detect_query_rate50).bestMatch;
  let matches_70 = stringSimilarity.findBestMatch(text, new_detect_query_rate70).bestMatch;
  if (matches_50.rating > 0.5 || matches_70.rating > 0.7) {
    const data = await requestAPI(apiHost, 'api/chart', '?type=new', 'GET');
    const message = jsonToString(data, "new");
    await sendMessage(message, channel);
    return;
  }
  const list_detect_query_rate50 = ['오늘', '멜론', '멜론차트', '전체차트', 'TOP'];
  const list_detect_query_rate70 = ['차트', '차트목록'];
  matches_50 = stringSimilarity.findBestMatch(text, list_detect_query_rate50).bestMatch;
  matches_70 = stringSimilarity.findBestMatch(text, list_detect_query_rate70).bestMatch;
  if(matches_50.rating > 0.5 || matches_70.rating > 0.7){
    const data = await requestAPI(apiHost, 'api/chart', '?type=all', 'GET');
    const {str, str2} = jsonToString(data, "all");
    await sendMessage(str, channel);
    await sendMessage(str2, channel);
    return;
  }

  const hot_detect_query_rate50 = ['급상승', '핫', '상승', '순위상승'];
  matches_50 = stringSimilarity.findBestMatch(text, hot_detect_query_rate50).bestMatch;
  if(matches_50.rating > 0.5){
    console.log('급상승');
    const data = await requestAPI(apiHost, 'api/chart', '?type=hot', 'GET');
    const str = jsonToString(data, "hot");
    await sendMessage(str, channel);
  }
});

schedule.scheduleJob('0 0 0 * * *', async () => {
  const data = await requestAPI(apiHost, 'api/chart', '', 'POST')
  const message = jsonToString(data, "update");
  if(message == "success"){
    let str = "차트가 업데이트 되었습니다!!"
    str += "\n- TOP100 차트 조회 : [오늘, 멜론, 멜론차트, 전체차트, TOP, 차트]"
    str += "\n- 새로 차트인한 신곡 조회 : [신곡, 최신, 차트인]" 
    str += "\n- 차트 급상승 목록 조회 : [급상승, 핫, 순위상승]" 
    sendMessage(str, process.env.BOT_CHANNAL_ID);
  }
});


(async () => {
  await rtm.start();
})();