const jsonToString = (data, type) => {
    const obj = JSON.parse(data);
    let str = "";
    if(type == "new"){
        const list = obj.ResultList;
        if(list.length == 0){
            str += "오늘의 차트인 신곡이 존재하지 않습니다."
            return str;
        }
        str += " === " + list[0].Regdate + " 기준 차트인 신곡 ====\n"
        for(let i = 0; i < list.length; i++){
            str += list[i].Rank + "위) " + list[i].Title + " : " + list[i].Singer + " \n";
        }
        return str;
    }
    else if(type == "update"){
        if(obj.result = "success"){
            str = "success";
        }
        else{
            str = "fail";
        }
        return str;
    }
    else if(type == "hot"){
        const list = obj.ResultList;
        if(list.length == 0){
            str += "오늘의 차트 급상승 곡이 존재하지 않습니다."
            return str;
        }
        str += " === " + list[0].Regdate + " 기준 차트 급상승 곡 ====\n"
        for(let i = 0; i < list.length; i++){
            str += list[i].Rank + "위(+" + list[i].Status + ") "  + list[i].Title + " : " + list[i].Singer + " \n";
        }
        return str;
    }
    else if(type == "all"){
        const list = obj.ResultList;
        str += " === " + list[0].Regdate + " 기준 차트 TOP 100 ====\n"
        let str2 = "";
        for(let i = 0; i < 50; i++){
            str += list[i].Rank + "위) " + list[i].Title + " : " + list[i].Singer + " \n";
        }
        for(let i = 50; i < 100; i++){
            str2 += list[i].Rank + "위) " + list[i].Title + " : " + list[i].Singer + " \n";
        }
        return {str, str2}
    }
    else{

    }
    return str;
}
module.exports = { jsonToString };