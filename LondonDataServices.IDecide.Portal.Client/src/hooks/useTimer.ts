import { useEffect, useState } from "react";

export function useTimer(totalSeconds: number = 60, resetKey: number = 0) {
    const [remainingSeconds, setRemainingSeconds] = useState(totalSeconds);
    const [timerExpired, setTimerExpired] = useState(false);
    const [startTime, setStartTime] = useState(new Date().getTime());

    useEffect(() => {
        setStartTime(new Date().getTime());
        setRemainingSeconds(totalSeconds);
        setTimerExpired(false);
    }, [resetKey, totalSeconds]);

    useEffect(() => {
        function getTimeRemaining(): void {
            const curretTime = new Date().getTime();
            const currentElapsedInSeconds = Math.floor((curretTime - startTime) / 1000);
            const remainingTime = totalSeconds - currentElapsedInSeconds;
            if (remainingTime <= 0) {
                setRemainingSeconds(0);
                setTimerExpired(true);
                clearInterval(interval);
            } else {
                setRemainingSeconds(remainingTime);
            }
        }

        const interval = setInterval(getTimeRemaining, 1000);
        return () => clearInterval(interval);
    }, [startTime, totalSeconds]);

    return {
        remainingSeconds, timerExpired
    };
}