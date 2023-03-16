using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RectSplitter
{
    
    public enum SplitBalanceMode
    {
        Random,
        Equal,
        LongestSide
    }

    public struct SplitParams
    {
        public float CutVertPercentage;
        public float CutOffsetMin;
        public float CutOffsetMax;

        public int MaxCuts;
        public int RandomCuts;
 
        public float MinRectSide;

        public SplitBalanceMode CutMode;
    }

    public static List<Rect> CutRect(Rect rect, SplitParams sparams, int cuts = 0)
    {
        var cut_rects = new List<Rect>();

        bool cut_vert = false;
        switch (sparams.CutMode)
        {
            case SplitBalanceMode.Random:
                cut_vert = Random.value < sparams.CutVertPercentage;
                break;
            case SplitBalanceMode.Equal:
                cut_vert = (cuts % 2) == 0;
                break;
            case SplitBalanceMode.LongestSide:
                cut_vert = rect.width > rect.height;
                break;
        }

        float offset = Random.value * (sparams.CutOffsetMax - sparams.CutOffsetMin);


        if (cut_vert) {
            var side = (rect.xMax - rect.xMin);
            var cut_point = rect.xMin + (side / 2) + (offset * side);


            var left_rect = new Rect(rect.xMin, rect.yMin, cut_point - rect.xMin, rect.yMax - rect.yMin);
            var right_rect = new Rect(cut_point, rect.yMin, rect.xMax - cut_point, rect.yMax - rect.yMin);

            if (left_rect.height > sparams.MinRectSide && left_rect.width > sparams.MinRectSide)
                cut_rects.Add(left_rect);

            if (right_rect.height > sparams.MinRectSide && right_rect.width > sparams.MinRectSide)
                cut_rects.Add(right_rect);

        } else {

            var side = (rect.yMax - rect.yMin);
            var cut_point = rect.yMin + (side / 2) + (offset * side);


            var bot_rect = new Rect(rect.xMin, rect.yMin, rect.xMax - rect.xMin, cut_point - rect.yMin);
            var top_rect = new Rect(rect.xMin , cut_point, rect.xMax - rect.xMin, rect.yMax - cut_point);


            if (bot_rect.height > sparams.MinRectSide && bot_rect.width > sparams.MinRectSide)
                cut_rects.Add(bot_rect);

            if (top_rect.height > sparams.MinRectSide && top_rect.width > sparams.MinRectSide)
                cut_rects.Add(top_rect);

        }

        return cut_rects;
    }


    public static List<Rect> Split(Rect rect, SplitParams sparams)
    {
        var rect_queue = new Queue<Rect>();

        rect_queue.Enqueue(rect);


        int cuts = 0;

        while (rect_queue.Count > 0 && (cuts < sparams.MaxCuts && (cuts < sparams.MaxCuts - sparams.RandomCuts)))
        {
            var r = rect_queue.Dequeue();

            var cut_rects = CutRect(r, sparams, cuts);
            
            foreach(var cut_rect in cut_rects)
                rect_queue.Enqueue(cut_rect);

            cuts++;
        }

        var rect_list = new List<Rect>(rect_queue);


        while (cuts < sparams.MaxCuts)
        {
            int ix = Random.Range(0, rect_list.Count);
            var r = rect_list[ix];
            rect_list.RemoveAt(ix);

            var cut_rects = CutRect(r, sparams, cuts);
            rect_list.AddRange(cut_rects);
            cuts++;
        }

        return rect_list;
    }


}
