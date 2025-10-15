import { useEffect, useState } from "react";
export function useTimer(totalSeconds: number = 60) {

    const [remainingSeconds, setRemainingSeconds] = useState(totalSeconds);
    const [timerExpired, setTimerExpired] = useState(false);
    const [startTime] = useState(new Date().getTime());

    useEffect(() => {
        function getTimeRemaining(): void {
            const curretTime = new Date().getTime();
            const currentElapsedInSeconds = Math.floor((curretTime - startTime) / 1000)
            const remainingTime = totalSeconds - currentElapsedInSeconds;
            if (remainingTime <= 0) {
                setRemainingSeconds(0);
                setTimerExpired(true);
                clearInterval(interval)
            } else {
                setRemainingSeconds(remainingTime);
            }
        }

        const interval = setInterval(getTimeRemaining, 1000);
        return () => clearInterval(interval);
    }, [startTime, totalSeconds])

    return {
        remainingSeconds, timerExpired
    }
}