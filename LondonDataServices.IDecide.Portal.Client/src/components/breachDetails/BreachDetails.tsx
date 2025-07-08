export const BreachDetails = () => {
    return <>
        <p>NHS England approved version 1.0 the following breach reporting guidance in 2017.</p>
        <p>The guidance defines what constitues a breach of patient confidentiality when performing re-identification.</p>
        <p>
            Our DSCRO re-identification partner (NECS) will monitor all activity and reserve the right to raise a breach
            report to NHS England should they detect any requests in excess of the following thresholds:
        </p>

        <ol>
            <li>Re-identification requests on more than 1 occasion between the hours of 9pm and 6am.</li>
            <li>Re-identification request is undertaken during an organisations/ GP practice non-working days.</li>
            <li>More than 100 successful Reidentification requests (requests, not patients) in one day</li>
            <li>Multiple reidentification requests from one user, totalling over 300 different patients, over a 24 hour period</li>
            <li>Multiple reidentification requests from one user, totalling over 500 different patients, over a 7 day period</li>
            <li>Re-identification of over 1000 distinct NHS numbers from the one user over a reporting period </li>
            <li>Multiple reidentification requests from the same organisation/GP Practice, totalling over 750 different patients</li>
            <li>Multiple requests from the same organisation/ GP Practice, totalling over 6% of the organisation/GP practice size over  a 7 day period</li>
            <li>Re-identification of distinct NHS numbers over 6% of GP practice size over a reporting period*</li>
            <li>Re-identification of distinct NHS numbers over 6% of GP practice size by CCG over a reporting period*</li>
            <li>Where an individual NHS number is identified 3 times or more over a reporting period*</li>
        </ol>

        <p>*Reporting period = within a month</p>
    </>
}